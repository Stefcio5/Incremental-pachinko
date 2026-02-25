using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using BreakInfinity;
using UnityEngine;

public class PlayerPrefsDataRepository : IDataRepository
{
    private const string SaveKey = "GameData";
    private const string BackupSaveKey = "GameData_Backup";
    private const int CurrentVersion = 3;

    private GameData _cachedData;
    private bool _hasCachedData;

    private readonly List<ISaveModule> _saveModules = new();
    private readonly ISaveSerializer _serializer;
    private readonly ISaveCompressor _compressor;

    public PlayerPrefsDataRepository()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _compressor = new GZipCompressor();
#else
        _compressor = new NoOpCompressor();
#endif
        _serializer = new JsonSaveSerializer();

        RegisterSaveModule(new CoreGameDataModule());
        RegisterSaveModule(new UpgradeSaveModule());
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
            CreateBackup();

            var saveContainer = SerializeModules();
            string json = _serializer.Serialize(saveContainer);
            string compressed = _compressor.Compress(json);

            PlayerPrefs.SetString(SaveKey, compressed);

            ValidateSave(compressed);
            UpdateCache(data);

#if !UNITY_WEBGL || UNITY_EDITOR
            PlayerPrefs.Save();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}\n{e.StackTrace}");
            RestoreFromBackup();
            throw;
        }
    }

    public GameData Load()
    {
        if (_hasCachedData)
            return CloneGameData(_cachedData);

        if (!PlayerPrefs.HasKey(SaveKey))
            return CreateDefaultData();

        try
        {
            string compressed = PlayerPrefs.GetString(SaveKey);
            string json = _compressor.Decompress(compressed);

            GameData gameData = TryLoadModularFormat(json) ?? LoadLegacyFormat(json);

            ValidateLoadedData(gameData);
            UpdateCache(gameData);

            return gameData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return TryRestoreFromBackup() ?? CreateDefaultData();
        }
    }

    public void ClearCache()
    {
        _hasCachedData = false;
        _cachedData = null;
    }

    #region Private Methods

    private void CreateBackup()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.SetString(BackupSaveKey, PlayerPrefs.GetString(SaveKey));
        }
    }

    private void RestoreFromBackup()
    {
        if (PlayerPrefs.HasKey(BackupSaveKey))
        {
            PlayerPrefs.SetString(SaveKey, PlayerPrefs.GetString(BackupSaveKey));
            Debug.LogWarning("Restored from backup");
        }
    }

    private GameData TryRestoreFromBackup()
    {
        if (!PlayerPrefs.HasKey(BackupSaveKey))
            return null;

        try
        {
            Debug.LogWarning("Attempting backup restore");
            string backupJson = PlayerPrefs.GetString(BackupSaveKey);
            string decompressed = _compressor.Decompress(backupJson);

            var serializableData = JsonUtility.FromJson<SerializableGameData>(decompressed);
            return serializableData?.ToGameData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Backup restore failed: {e.Message}");
            return null;
        }
    }

    private SaveContainer SerializeModules()
    {
        var moduleDataList = new List<ModuleSaveData>();

        foreach (var module in _saveModules)
        {
            try
            {
                moduleDataList.Add(new ModuleSaveData
                {
                    moduleName = module.ModuleName,
                    version = module.Version,
                    data = module.Serialize()
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize {module.ModuleName}: {e.Message}");
            }
        }

        return new SaveContainer
        {
            version = CurrentVersion,
            modules = moduleDataList.ToArray()
        };
    }

    private GameData TryLoadModularFormat(string json)
    {
        try
        {
            var saveContainer = JsonUtility.FromJson<SaveContainer>(json);
            if (saveContainer?.modules == null)
                return null;

            DeserializeModules(saveContainer);
            return DataController.Instance.CurrentGameData;
        }
        catch
        {
            return null;
        }
    }

    private void DeserializeModules(SaveContainer saveContainer)
    {
        foreach (var moduleData in saveContainer.modules)
        {
            var module = _saveModules.FirstOrDefault(m => m.ModuleName == moduleData.moduleName);

            if (module == null)
            {
                Debug.LogWarning($"Unknown save module: {moduleData.moduleName}");
                continue;
            }

            try
            {
                if (moduleData.version != module.Version)
                {
                    module.OnMigrate(moduleData.version, module.Version);
                }

                module.Deserialize(moduleData.data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize {moduleData.moduleName}: {e.Message}");
            }
        }
    }

    private GameData LoadLegacyFormat(string json)
    {
        Debug.LogWarning("Loading legacy save format");
        var serializableData = JsonUtility.FromJson<SerializableGameData>(json);

        if (serializableData == null)
            throw new Exception("Deserialized data is null");

        return serializableData.ToGameData();
    }

    private void ValidateSave(string savedData)
    {
        if (PlayerPrefs.GetString(SaveKey) != savedData)
        {
            throw new Exception("Save validation failed - data mismatch");
        }
    }

    private void ValidateLoadedData(GameData data)
    {
        if (data.points < 0) data.points = 0;
        if (data.totalPoints < 0) data.totalPoints = 0;
        if (data.prestigePoints < 0) data.prestigePoints = 0;
        data.upgradeLevels ??= new Dictionary<string, BigDouble>();
    }

    private void UpdateCache(GameData data)
    {
        _cachedData = CloneGameData(data);
        _hasCachedData = true;
    }

    private GameData CreateDefaultData()
    {
        var defaultData = new GameData();
        UpdateCache(defaultData);
        return defaultData;
    }

    private GameData CloneGameData(GameData original)
    {
        return new GameData(
            original.points,
            original.totalPoints,
            original.prestigePoints,
            new Dictionary<string, BigDouble>(original.upgradeLevels)
        );
    }

    #endregion
}

#region Compression Interfaces

public interface ISaveCompressor
{
    string Compress(string data);
    string Decompress(string data);
}

public class NoOpCompressor : ISaveCompressor
{
    public string Compress(string data) => data;
    public string Decompress(string data) => data;
}

public class GZipCompressor : ISaveCompressor
{
    public string Compress(string data)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Compression failed: {e.Message}");
            return data;
        }
    }

    public string Decompress(string data)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(data);
            using var input = new MemoryStream(bytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
        catch
        {
            return data; // Fallback - probably uncompressed
        }
    }
}

public interface ISaveSerializer
{
    string Serialize(object obj);
    T Deserialize<T>(string data);
}

public class JsonSaveSerializer : ISaveSerializer
{
    public string Serialize(object obj) => JsonUtility.ToJson(obj);
    public T Deserialize<T>(string data) => JsonUtility.FromJson<T>(data);
}

#endregion


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