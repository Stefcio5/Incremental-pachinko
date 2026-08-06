using System;
using System.Collections.Generic;
using BreakInfinity;

[System.Serializable]
public class SerializableGameData
{
    public string points;
    public string totalPoints;
    public string prestigePoints;
    public UpgradeLevelData[] upgradeLevels;
    public int version;
    public long timestamp;

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
