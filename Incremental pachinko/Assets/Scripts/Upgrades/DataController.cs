using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class DataController : PersistentSingleton<DataController>
{
    [SerializeField] private GameData _currentGameData = new GameData();
    public GameData CurrentGameData
    {
        get => _currentGameData;
        private set => _currentGameData = value;
    }
    public event Action OnDataChanged;

    [SerializeField] private PlayerPrefsDataRepository _dataRepository;

    private bool _isSaving;

    public BigDouble PrestigePoints;

    protected override void Awake()
    {
        base.Awake();
        if (_dataRepository == null) _dataRepository = new PlayerPrefsDataRepository();
    }
    private void Start()
    {
        LoadData();
    }

    public void AddPoints(BigDouble amount)
    {
        if (amount <= 0) return;
        CurrentGameData.points += amount;
        SaveDataAndNotify();
    }

    public bool SpendPoints(BigDouble amount)
    {
        if (CurrentGameData.points < amount) return false;
        CurrentGameData.points -= amount;
        SaveDataAndNotify();
        return true;
    }

    public bool SpendPrestigePoints(BigDouble amount)
    {
        if (CurrentGameData.prestigePoints < amount) return false;
        CurrentGameData.prestigePoints -= amount;
        SaveDataAndNotify();
        return true;
    }

    public void PrestigeGame()
    {
        CurrentGameData.prestigePoints += CalculatePrestige();
        CurrentGameData.points = 0;
        ResetGameData();
    }

    private BigDouble CalculatePrestige() => BigDouble.Sqrt(CurrentGameData.points) / 2;

    [ContextMenu("Reset Game Data")]
    private void ResetGameData()
    {
        foreach (var key in CurrentGameData.upgradeLevels.Keys.ToList())
        {
            CurrentGameData.upgradeLevels[key] = 0;
        }
        CurrentGameData.points = 0;
        UpgradeManager.Instance.ResetUpgrades();
        SaveDataAndNotify();
    }

    private void LoadData()
    {
        try
        {
            CurrentGameData = _dataRepository.Load();
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
            _dataRepository.Save(CurrentGameData);
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

    private void SaveDataAndNotify()
    {
        SaveData();
        OnDataChanged?.Invoke();
    }
}