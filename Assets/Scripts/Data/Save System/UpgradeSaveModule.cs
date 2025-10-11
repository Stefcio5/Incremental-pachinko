using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class UpgradeSaveModule : ISaveModule
{
    public string ModuleName => "Upgrades";
    public int Version => 1;

    public string Serialize()
    {
        var upgradeLevels = DataController.Instance.CurrentGameData.upgradeLevels;
        var upgradeArray = new UpgradeLevelData[upgradeLevels.Count];
        int index = 0;

        foreach (var kvp in upgradeLevels)
        {
            upgradeArray[index] = new UpgradeLevelData
            {
                id = kvp.Key,
                level = kvp.Value.ToString()
            };
            index++;
        }

        var data = new UpgradeModuleData { upgradeLevels = upgradeArray };
        return JsonUtility.ToJson(data);
    }

    public void Deserialize(string data)
    {
        var moduleData = JsonUtility.FromJson<UpgradeModuleData>(data);
        if (moduleData?.upgradeLevels != null)
        {
            var upgradeDict = new Dictionary<string, BigDouble>();

            foreach (var upgrade in moduleData.upgradeLevels)
            {
                if (!string.IsNullOrEmpty(upgrade.id))
                {
                    upgradeDict[upgrade.id] = SafeParseBigDouble(upgrade.level);
                }
            }

            DataController.Instance.CurrentGameData.upgradeLevels = upgradeDict;
        }
    }

    public void OnMigrate(int fromVersion, int toVersion)
    {
        // Handle upgrade data migrations
    }

    private static BigDouble SafeParseBigDouble(string value)
    {
        try
        {
            return string.IsNullOrEmpty(value) ? BigDouble.Zero : BigDouble.Parse(value);
        }
        catch (Exception)
        {
            return BigDouble.Zero;
        }
    }
}

[Serializable]
public class UpgradeModuleData
{
    public UpgradeLevelData[] upgradeLevels;
}

[Serializable]
public struct UpgradeLevelData
{
    public string id;
    public string level;
}