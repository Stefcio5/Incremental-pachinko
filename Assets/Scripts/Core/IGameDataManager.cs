using System;
using BreakInfinity;
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IGameDataManager : IManager
{
    GameData CurrentGameData { get; }
    event Action OnDataChanged;
    event Action OnPrestige;

    void AddPoints(BigDouble amount);
    bool SpendPoints(BigDouble amount);
    bool SpendPrestigePoints(BigDouble amount);
    BigDouble CalculatePrestige();
    BigDouble PointsToNextPrestige();
    void PrestigeGame();
    void SaveData();
    UniTask SaveDataAsync(CancellationToken cancellationToken = default);
    UniTask LoadDataAsync(CancellationToken cancellationToken = default);
}
