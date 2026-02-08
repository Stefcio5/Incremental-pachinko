using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class DataController : PersistentSingleton<DataController>
{
    [field: SerializeField]
    public GameData CurrentGameData { get; private set; }
    private SaveSystem _saveSystem;
    public event Action OnDataChanged;
    private bool _isSaving;

    [SerializeField] private FlyweightRuntimeSetSO _flyweightRuntimeSet;
    [SerializeField] private BigDoubleSO _prestigePointsMultiplier;

    protected override void Awake()
    {
        base.Awake();
        _saveSystem = new SaveSystem(new PlayerPrefsDataRepository());
    }
    private void Start()
    {
        LoadData();
    }

    public void AddPoints(BigDouble amount)
    {
        if (amount <= 0) return;
        CurrentGameData.points += amount;
        CurrentGameData.totalPoints += amount;
        OnDataChanged?.Invoke();
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
        ResetGameDataOnPrestige();
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
        _flyweightRuntimeSet.ReturnAllFlyweightsToPool();
        CurrentGameData = new GameData();
        PlayerPrefs.DeleteAll();
        UpgradeManager.Instance.ResetAllUpgrades();
        OnDataChanged?.Invoke();
    }

    private void LoadData()
    {
        try
        {
            CurrentGameData = _saveSystem.Load();
            ValidateLoadedData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}, using default data");
            CurrentGameData = new GameData();
        }
        OnDataChanged?.Invoke();
    }

    private void ValidateLoadedData()
    {
        if (CurrentGameData.points < 0) CurrentGameData.points = 0;
        if (CurrentGameData.prestigePoints < 0) CurrentGameData.prestigePoints = 0;
        CurrentGameData.upgradeLevels ??= new Dictionary<string, BigDouble>();
    }

    public void SaveData()
    {
        if (_isSaving) return;
        try
        {
            _isSaving = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        // Na WebGL użyj batch save
        if (WebGLOptimizedSaveSystem.Instance != null)
        {
            WebGLOptimizedSaveSystem.Instance.ScheduleSave(CurrentGameData);
        }
        else
        {
            _saveSystem.Save(CurrentGameData);
        }
#else
            _saveSystem.Save(CurrentGameData);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    void OnApplicationQuit()
    {
        _saveSystem.Save(CurrentGameData);
    }
}