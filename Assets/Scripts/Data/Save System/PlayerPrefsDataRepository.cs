using System;
using UnityEngine;

public class PlayerPrefsDataRepository : IDataRepository
{
    private const string SaveKey = "GameData";

    private readonly ISaveSerializer _serializer;
    private readonly ISaveCompressor _compressor;
    private readonly PlatformSaveConfiguration _platformConfig;
    private readonly SaveBackupManager _backupManager;
    private readonly SaveDataCache _cache;
    private readonly SaveDataValidator _validator;
    private readonly SaveModuleCoordinator _moduleCoordinator;

    public PlayerPrefsDataRepository()
    {
        _platformConfig = new PlatformSaveConfiguration();
        _compressor = _platformConfig.CreateCompressor();
        _serializer = _platformConfig.CreateSerializer();

        _cache = new SaveDataCache();
        _validator = new SaveDataValidator();
        _moduleCoordinator = new SaveModuleCoordinator();
        _backupManager = new SaveBackupManager(_serializer, _compressor, _moduleCoordinator);

        RegisterSaveModule(new CoreGameDataModule());
        RegisterSaveModule(new UpgradeSaveModule());
    }

    public void RegisterSaveModule(ISaveModule module)
    {
        _moduleCoordinator.RegisterModule(module);
    }

    public void Save(GameData data)
    {
        try
        {
            _backupManager.CreateBackup();

            var saveContainer = _moduleCoordinator.SerializeModules();
            string json = _serializer.Serialize(saveContainer);
            string compressed = _compressor.Compress(json);

            PlayerPrefs.SetString(SaveKey, compressed);

            _validator.ValidateSave(compressed);
            _cache.Set(data);

            if (_platformConfig.ShouldCallPlayerPrefsSave)
            {
                PlayerPrefs.Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}\n{e.StackTrace}");
            _backupManager.RestoreFromBackup();
            throw;
        }
    }

    public GameData Load()
    {
        if (_cache.HasCached)
            return _cache.Get();

        if (!PlayerPrefs.HasKey(SaveKey))
            return CreateDefaultData();

        try
        {
            string compressed = PlayerPrefs.GetString(SaveKey);
            string json = _compressor.Decompress(compressed);

            var saveContainer = _serializer.Deserialize<SaveContainer>(json);
            if (saveContainer?.modules == null)
                throw new Exception("Invalid save data format");

            _moduleCoordinator.DeserializeModules(saveContainer);
            GameData gameData = DataController.Instance.CurrentGameData;

            _validator.ValidateLoadedData(gameData);
            _cache.Set(gameData);

            return gameData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return _backupManager.TryRestoreFromBackup() ?? CreateDefaultData();
        }
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    private GameData CreateDefaultData()
    {
        var defaultData = new GameData();
        _cache.Set(defaultData);
        return defaultData;
    }
}