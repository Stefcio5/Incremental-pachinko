using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BreakInfinity;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataController : PersistentSingleton<DataController>, IGameSystem
{
    [field: SerializeField]
    public GameData CurrentGameData { get; private set; }

    private SaveSystem _saveSystem;

    public event Action OnDataChanged;
    public event Action OnPrestige;
    public event Action OnGameReset;
    public event Action OnSystemInitialized;

    private bool _isSaving;
    private bool _isInitialized;

    [SerializeField] private FlyweightRuntimeSetSO _flyweightRuntimeSet;
    [SerializeField] private BigDoubleSO _prestigePointsMultiplier;

    public string SystemName => "DataController";
    public bool IsInitialized => _isInitialized;

    protected override void Awake()
    {
        base.Awake();
        _saveSystem = new SaveSystem(new PlayerPrefsDataRepository());
    }

    public async UniTask InitializeAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            Debug.LogWarning($"[{SystemName}] Already initialized");
            return;
        }

        progress?.Report(0f);

        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

        progress?.Report(0.3f);

        await LoadDataAsync(cancellationToken);

        progress?.Report(1f);

        _isInitialized = true;
        OnSystemInitialized?.Invoke();

        Debug.Log($"[{SystemName}] Initialization complete");
    }

    private async UniTask LoadDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            CurrentGameData = _saveSystem.Load();
            ValidateLoadedData();

            Debug.Log($"[{SystemName}] Data loaded successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{SystemName}] Failed to load data: {e.Message}, using default data");
            CurrentGameData = new GameData();
        }

        OnDataChanged?.Invoke();
    }

    public void AddPoints(BigDouble amount)
    {
        if (amount <= 0) return;
        CurrentGameData.points += amount;
        CurrentGameData.totalPoints += amount;
        OnDataChanged?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddPoints(1000000);
        }
    }

    public bool SpendPoints(BigDouble amount)
    {
        if (CurrentGameData.points < amount) return false;
        CurrentGameData.points -= amount;
        OnDataChanged?.Invoke();
        return true;
    }

    public bool SpendPrestigePoints(BigDouble amount)
    {
        if (CurrentGameData.prestigePoints < amount) return false;
        CurrentGameData.prestigePoints -= amount;
        OnDataChanged?.Invoke();
        return true;
    }

    public void PrestigeGame()
    {
        _flyweightRuntimeSet.ReturnAllFlyweightsToPool();
        CurrentGameData.prestigePoints += CalculatePrestige();
        CurrentGameData.points = 0;
        OnPrestige?.Invoke();
        ResetGameDataOnPrestige();
        OnGameReset?.Invoke();
    }

    //TODO: Change magic numbers
    public BigDouble CalculatePrestige() => BigDouble.Floor(BigDouble.Sqrt(CurrentGameData.totalPoints / 1000000000)) * _prestigePointsMultiplier.DisplayValue;
    public BigDouble PointsToNextPrestige()
    {
        BigDouble prestigePointsToAdd = CalculatePrestige() / _prestigePointsMultiplier.DisplayValue;
        return BigDouble.Pow(prestigePointsToAdd + 1, 2) * 1000000000 - CurrentGameData.totalPoints;
    }


    [ContextMenu("Reset Game Data On Prestige")]
    private void ResetGameDataOnPrestige()
    {
        UpgradeManager.Instance.ResetUpgradesExceptPrestige();

        CurrentGameData.points = 0;
        CurrentGameData.totalPoints = 0;
        OnDataChanged?.Invoke();
    }
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        _flyweightRuntimeSet.ReturnAllFlyweightsToPool();
        CurrentGameData = new GameData();
        UpgradeManager.Instance.ResetAllUpgrades();
        OnGameReset?.Invoke();
        OnDataChanged?.Invoke();
    }

    private void ValidateLoadedData()
    {
        if (CurrentGameData.points < 0) CurrentGameData.points = 0;
        if (CurrentGameData.prestigePoints < 0) CurrentGameData.prestigePoints = 0;
        CurrentGameData.upgradeLevels ??= new Dictionary<string, BigDouble>();
    }

    /// <summary>
    /// Save game data synchronously.
    /// Consider using SaveDataAsync for better performance.
    /// </summary>
    public void SaveData()
    {
        if (_isSaving) return;

        try
        {
            _isSaving = true;
            _saveSystem.Save(CurrentGameData);
        }
        catch (Exception e)
        {
            Debug.LogError($"[{SystemName}] Failed to save data: {e.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    /// <summary>
    /// Save game data asynchronously (recommended).
    /// </summary>
    public async UniTask SaveDataAsync(CancellationToken cancellationToken = default)
    {
        if (_isSaving) return;

        try
        {
            _isSaving = true;

            // Move to background thread for serialization if needed
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            _saveSystem.Save(CurrentGameData);

            Debug.Log($"[{SystemName}] Data saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{SystemName}] Failed to save data: {e.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private void OnApplicationQuit()
    {
        if (_isInitialized)
        {
            _saveSystem.Save(CurrentGameData);
        }
    }
}