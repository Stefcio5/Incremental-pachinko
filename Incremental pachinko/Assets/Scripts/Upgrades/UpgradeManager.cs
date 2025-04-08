using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class UpgradeManager : PersistentSingleton<UpgradeManager>
{
    [SerializeField] private List<UpgradeConfig> _upgradeConfigs = new();
    private readonly Dictionary<UpgradeType, List<Upgrade>> _upgrades = new();
    public Dictionary<string, Upgrade> upgradeMap = new();

    public event Action OnUpgradesChanged;
    public event Action OnInitialized;

    private bool _initialized;
    public bool Initialized => _initialized;

    protected override void Awake()
    {
        base.Awake();
        //StartCoroutine(InitializeWhenReady());
    }

    private void Start()
    {
        //StartCoroutine(InitializeWhenReady());
        Initialize(_upgradeConfigs, DataController.Instance);
    }

    // Obsolete
    private IEnumerator InitializeWhenReady()
    {
        Debug.Log("UpgradeManager waiting for DataController...");
        yield return new WaitUntil(() => DataController.Instance != null);

        if (!_initialized)
        {
            try
            {
                DataController.Instance.OnDataChanged += HandleDataChanged;
                Initialize(_upgradeConfigs, DataController.Instance);
                _initialized = true;
                OnInitialized?.Invoke();
                OnUpgradesChanged?.Invoke();
                Debug.Log("UpgradeManager initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize UpgradeManager: {e.Message}");
            }
        }
    }

    public void Initialize(IEnumerable<UpgradeConfig> configs, DataController data)
    {
        if (configs == null || data == null)
        {
            throw new ArgumentNullException("configs or data is null");
        }

        _upgrades.Clear();
        upgradeMap.Clear();

        foreach (var config in configs)
        {
            if (config == null) continue;

            BigDouble initialLevel = data.CurrentGameData.upgradeLevels.GetValueOrDefault(config.upgradeName, 0);
            var upgrade = new Upgrade(config, initialLevel);
            upgrade.OnLevelChanged += (u) => SaveUpgrade(config.upgradeName, u.CurrentLevel, data);

            if (!_upgrades.ContainsKey(config.upgradeType))
            {
                _upgrades[config.upgradeType] = new List<Upgrade>();
            }

            _upgrades[config.upgradeType].Add(upgrade);
            upgradeMap[config.upgradeName] = upgrade;
        }
        OnUpgradesChanged?.Invoke();
        _initialized = true;
        Debug.Log("Upgrade Manager Initialized");
    }

    private void SaveUpgrade(string id, BigDouble level, DataController data)
    {
        data.CurrentGameData.upgradeLevels[id] = level;
        data.SaveData();
    }

    public IEnumerable<Upgrade> GetUpgrades(UpgradeType type) =>
        _upgrades.TryGetValue(type, out var upgrades) ? upgrades : Enumerable.Empty<Upgrade>();

    public Upgrade GetUpgrade(string upgradeName) =>
        upgradeMap.TryGetValue(upgradeName, out var upgrade) ? upgrade : null;

    public void HandleDataChanged()
    {
        foreach (var upgrade in upgradeMap.Values)
        {
            upgrade.UpdateLevel(DataController.Instance.CurrentGameData.upgradeLevels.GetValueOrDefault(upgrade.config.upgradeName, 0));
            Debug.Log("Upgrade level updated: " + upgrade.config.upgradeName + " to " + upgrade.CurrentLevel);
        }
        OnUpgradesChanged?.Invoke();
    }
}