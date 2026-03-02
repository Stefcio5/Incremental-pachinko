using System.Collections.Generic;
using BreakInfinity;

public class SaveDataCache
{
    private GameData _cachedData;
    private bool _hasCachedData;

    public bool HasCached => _hasCachedData;

    public GameData Get()
    {
        return _hasCachedData ? CloneGameData(_cachedData) : null;
    }

    public void Set(GameData data)
    {
        _cachedData = CloneGameData(data);
        _hasCachedData = true;
    }

    public void Clear()
    {
        _hasCachedData = false;
        _cachedData = null;
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
}
