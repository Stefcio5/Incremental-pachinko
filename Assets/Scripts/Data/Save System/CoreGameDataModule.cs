using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

public class CoreGameDataModule : ISaveModule
{
    public string ModuleName => "CoreGameData";
    public int Version => 1;

    public string Serialize()
    {
        var data = new CoreGameModuleData
        {
            points = DataController.Instance.CurrentGameData.points.ToString(),
            totalPoints = DataController.Instance.CurrentGameData.totalPoints.ToString(),
            prestigePoints = DataController.Instance.CurrentGameData.prestigePoints.ToString()
        };
        return JsonUtility.ToJson(data);
    }

    public void Deserialize(string data)
    {
        var moduleData = JsonUtility.FromJson<CoreGameModuleData>(data);
        if (moduleData != null)
        {
            DataController.Instance.CurrentGameData.points = BigDouble.SafeParseBigDouble(moduleData.points);
            DataController.Instance.CurrentGameData.totalPoints = BigDouble.SafeParseBigDouble(moduleData.totalPoints);
            DataController.Instance.CurrentGameData.prestigePoints = BigDouble.SafeParseBigDouble(moduleData.prestigePoints);
        }
    }

    public void OnMigrate(int fromVersion, int toVersion)
    {
        // Handle migrations for core data if needed
    }
}

[Serializable]
public class CoreGameModuleData
{
    public string points;
    public string totalPoints;
    public string prestigePoints;
}