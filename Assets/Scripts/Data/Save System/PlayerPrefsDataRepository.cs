using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BreakInfinity;
using UnityEngine;

public class PlayerPrefsDataRepository : IDataRepository
{
    private const string SaveKey = "GameData";
    private const string BackupSaveKey = "GameData_Backup";
    private const int CurrentVersion = 3;

    // Cache to avoid multiple deserialization
    private GameData _cachedData;
    private bool _hasCachedData;

    // Modular save system
    private readonly List<ISaveModule> _saveModules = new List<ISaveModule>();

    public PlayerPrefsDataRepository()
    {
        // Register save modules
        RegisterSaveModule(new CoreGameDataModule());
        RegisterSaveModule(new UpgradeSaveModule());
        // RegisterSaveModule(new SkillTreeSaveModule()); // Easy to add new modules
    }

    public void RegisterSaveModule(ISaveModule module)
    {
        if (!_saveModules.Any(m => m.ModuleName == module.ModuleName))
        {
            _saveModules.Add(module);
        }
    }

    public void Save(GameData data)
    {
        try
        {
            // First save backup of current data
            if (PlayerPrefs.HasKey(SaveKey))
            {
                string currentData = PlayerPrefs.GetString(SaveKey);
                PlayerPrefs.SetString(BackupSaveKey, currentData);
            }

            // Use modular save system
            var moduleDataList = new List<ModuleSaveData>();

            foreach (var module in _saveModules)
            {
                try
                {
                    var moduleData = new ModuleSaveData
                    {
                        moduleName = module.ModuleName,
                        version = module.Version,
                        data = module.Serialize()
                    };
                    moduleDataList.Add(moduleData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to serialize module {module.ModuleName}: {e.Message}");
                }
            }

            var saveContainer = new SaveContainer
            {
                version = CurrentVersion,
                modules = moduleDataList.ToArray()
            };

            string json = JsonUtility.ToJson(saveContainer);

            // Compression for WebGL
#if UNITY_WEBGL && !UNITY_EDITOR
            json = CompressString(json);
#endif

            PlayerPrefs.SetString(SaveKey, json);

            // Save validation
            if (PlayerPrefs.GetString(SaveKey) != json)
            {
                throw new Exception("Save validation failed");
            }

            // Cache new data
            _cachedData = data;
            _hasCachedData = true;

            // Batch save for WebGL - will be called automatically
#if !UNITY_WEBGL || UNITY_EDITOR
            PlayerPrefs.Save();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");

            // Restore backup if failed
            if (PlayerPrefs.HasKey(BackupSaveKey))
            {
                PlayerPrefs.SetString(SaveKey, PlayerPrefs.GetString(BackupSaveKey));
            }
            throw;
        }
    }

    public GameData Load()
    {
        // Return cache if available
        if (_hasCachedData)
        {
            return new GameData(_cachedData.points, _cachedData.totalPoints,
                              _cachedData.prestigePoints, new Dictionary<string, BigDouble>(_cachedData.upgradeLevels));
        }

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            var defaultData = new GameData();
            _cachedData = defaultData;
            _hasCachedData = true;
            return defaultData;
        }

        try
        {
            string json = PlayerPrefs.GetString(SaveKey);

            // Decompression for WebGL
#if UNITY_WEBGL && !UNITY_EDITOR
            json = DecompressString(json);
#endif

            // Try new modular format first
            try
            {
                var saveContainer = JsonUtility.FromJson<SaveContainer>(json);
                if (saveContainer?.modules != null)
                {
                    return LoadModularData(saveContainer);
                }
            }
            catch
            {
                // Fall back to legacy format
                Debug.LogWarning("Failed to load modular save format, trying legacy format");
            }

            // Legacy format fallback
            var serializableData = JsonUtility.FromJson<SerializableGameData>(json);
            if (serializableData == null)
                throw new Exception("Deserialized data is null");

            var gameData = serializableData.ToGameData();
            ValidateLoadedData(gameData);

            // Cache loaded data
            _cachedData = gameData;
            _hasCachedData = true;

            return gameData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");

            // Attempt to restore from backup
            if (PlayerPrefs.HasKey(BackupSaveKey))
            {
                try
                {
                    Debug.LogWarning("Attempting to restore from backup");
                    string backupJson = PlayerPrefs.GetString(BackupSaveKey);
                    var backupData = JsonUtility.FromJson<SerializableGameData>(backupJson);
                    return backupData.ToGameData();
                }
                catch (Exception backupException)
                {
                    Debug.LogError($"Backup restore failed: {backupException.Message}");
                }
            }

            return new GameData();
        }
    }

    private GameData LoadModularData(SaveContainer saveContainer)
    {
        var gameData = new GameData();

        foreach (var moduleData in saveContainer.modules)
        {
            var module = _saveModules.FirstOrDefault(m => m.ModuleName == moduleData.moduleName);
            if (module != null)
            {
                try
                {
                    // Handle version migration if needed
                    if (moduleData.version != module.Version)
                    {
                        module.OnMigrate(moduleData.version, module.Version);
                    }

                    module.Deserialize(moduleData.data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to deserialize module {moduleData.moduleName}: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Unknown save module: {moduleData.moduleName}");
            }
        }

        // Return the loaded data from DataController
        return DataController.Instance.CurrentGameData;
    }

    private void ValidateLoadedData(GameData data)
    {
        if (data.points < 0) data.points = 0;
        if (data.totalPoints < 0) data.totalPoints = 0;
        if (data.prestigePoints < 0) data.prestigePoints = 0;
        data.upgradeLevels ??= new Dictionary<string, BigDouble>();
    }

    private SerializableGameData MigrateData(SerializableGameData oldData)
    {
        // Example migration from version 1 to 2
        if (oldData.version == 1)
        {
            // Add new fields or transform existing ones
            oldData.version = 2;
        }
        return oldData;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private string CompressString(string text)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            // Simple compression algorithm - in reality use GZip
            return Convert.ToBase64String(data);
        }
        catch
        {
            return text; // Fallback without compression
        }
    }

    private string DecompressString(string compressedText)
    {
        try
        {
            byte[] data = Convert.FromBase64String(compressedText);
            return Encoding.UTF8.GetString(data);
        }
        catch
        {
            return compressedText; // Fallback - probably uncompressed
        }
    }
#endif

    public void ClearCache()
    {
        _hasCachedData = false;
        _cachedData = null;
    }
}

[System.Serializable]
public class SerializableGameData
{
    public string points;
    public string totalPoints;
    public string prestigePoints;
    public UpgradeLevelData[] upgradeLevels;
    public int version;
    public long timestamp; // Unix timestamp

    [System.Serializable]
    public struct UpgradeLevelData
    {
        public string id;
        public string level;
    }

    public SerializableGameData()
    {
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public SerializableGameData(GameData data)
    {
        points = data.points.ToString();
        totalPoints = data.totalPoints.ToString();
        prestigePoints = data.prestigePoints.ToString();

        // Conversion of dictionary to array for better serialization
        upgradeLevels = new UpgradeLevelData[data.upgradeLevels.Count];
        int index = 0;
        foreach (var kvp in data.upgradeLevels)
        {
            upgradeLevels[index] = new UpgradeLevelData
            {
                id = kvp.Key,
                level = kvp.Value.ToString()
            };
            index++;
        }

        version = 2;
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public GameData ToGameData()
    {
        var upgradeDict = new Dictionary<string, BigDouble>();

        if (upgradeLevels != null)
        {
            foreach (var upgrade in upgradeLevels)
            {
                if (!string.IsNullOrEmpty(upgrade.id))
                {
                    upgradeDict[upgrade.id] = BigDouble.SafeParseBigDouble(upgrade.level);
                }
            }
        }

        return new GameData(
            BigDouble.SafeParseBigDouble(points),
            BigDouble.SafeParseBigDouble(totalPoints),
            BigDouble.SafeParseBigDouble(prestigePoints),
            upgradeDict
        );
    }
}

// Data structures for modular save system
[System.Serializable]
public class SaveContainer
{
    public int version;
    public ModuleSaveData[] modules;
}

[System.Serializable]
public class ModuleSaveData
{
    public string moduleName;
    public int version;
    public string data;
}