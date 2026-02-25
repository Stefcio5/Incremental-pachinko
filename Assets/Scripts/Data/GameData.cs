using System.Collections.Generic;
using BreakInfinity;

[System.Serializable]
public class GameData
{
    public BigDouble points = 0;
    public BigDouble totalPoints = 0;
    public BigDouble prestigePoints = 0;
    public Dictionary<string, BigDouble> upgradeLevels = new();

    public void UpgradeLevel(string upgradeId, BigDouble level)
    {
        if (upgradeLevels.ContainsKey(upgradeId))
            upgradeLevels[upgradeId] = level;
        else
            upgradeLevels.Add(upgradeId, level);
    }

    public BigDouble GetUpgradeLevel(string upgradeId)
    {
        return upgradeLevels.TryGetValue(upgradeId, out var level) ? level : 0;
    }

    public GameData()
    {
    }

    public GameData(BigDouble points, BigDouble totalPoints, BigDouble prestigePoints, Dictionary<string, BigDouble> upgradeLevels)
    {
        this.points = points;
        this.totalPoints = totalPoints;
        this.prestigePoints = prestigePoints;
        this.upgradeLevels = upgradeLevels;
    }
}

public interface IDataRepository
{
    void Save(GameData data);
    GameData Load();
    void ClearCache();
}
