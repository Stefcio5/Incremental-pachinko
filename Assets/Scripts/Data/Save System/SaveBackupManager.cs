using System;
using UnityEngine;

public class SaveBackupManager
{
    private const string SaveKey = "GameData";
    private const string BackupSaveKey = "GameData_Backup";

    private readonly ISaveSerializer _serializer;
    private readonly ISaveCompressor _compressor;
    private readonly SaveModuleCoordinator _moduleCoordinator;

    public SaveBackupManager(ISaveSerializer serializer, ISaveCompressor compressor, SaveModuleCoordinator moduleCoordinator)
    {
        _serializer = serializer;
        _compressor = compressor;
        _moduleCoordinator = moduleCoordinator;
    }

    public void CreateBackup()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.SetString(BackupSaveKey, PlayerPrefs.GetString(SaveKey));
        }
    }

    public void RestoreFromBackup()
    {
        if (PlayerPrefs.HasKey(BackupSaveKey))
        {
            PlayerPrefs.SetString(SaveKey, PlayerPrefs.GetString(BackupSaveKey));
            Debug.LogWarning("Restored from backup");
        }
    }

    public GameData TryRestoreFromBackup()
    {
        if (!PlayerPrefs.HasKey(BackupSaveKey))
            return null;

        try
        {
            Debug.LogWarning("Attempting backup restore");
            string backupCompressed = PlayerPrefs.GetString(BackupSaveKey);
            string backupJson = _compressor.Decompress(backupCompressed);

            var saveContainer = _serializer.Deserialize<SaveContainer>(backupJson);
            if (saveContainer?.modules == null)
                return null;

            _moduleCoordinator.DeserializeModules(saveContainer);
            return DataController.Instance.CurrentGameData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Backup restore failed: {e.Message}");
            return null;
        }
    }
}
