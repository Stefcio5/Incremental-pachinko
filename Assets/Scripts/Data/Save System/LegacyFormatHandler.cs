using System;
using UnityEngine;

public class LegacyFormatHandler
{
    private readonly ISaveSerializer _serializer;
    private readonly SaveModuleCoordinator _moduleCoordinator;

    public LegacyFormatHandler(ISaveSerializer serializer, SaveModuleCoordinator moduleCoordinator)
    {
        _serializer = serializer;
        _moduleCoordinator = moduleCoordinator;
    }

    public GameData TryLoad(string json)
    {
        return TryLoadModularFormat(json) ?? LoadLegacyFormat(json);
    }

    private GameData TryLoadModularFormat(string json)
    {
        try
        {
            var saveContainer = _serializer.Deserialize<SaveContainer>(json);
            if (saveContainer?.modules == null)
                return null;

            _moduleCoordinator.DeserializeModules(saveContainer);
            return DataController.Instance.CurrentGameData;
        }
        catch
        {
            return null;
        }
    }

    private GameData LoadLegacyFormat(string json)
    {
        Debug.LogWarning("Loading legacy save format");
        var serializableData = _serializer.Deserialize<SerializableGameData>(json);

        if (serializableData == null)
            throw new Exception("Deserialized data is null");

        return serializableData.ToGameData();
    }
}
