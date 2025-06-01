using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class UpgradeManager : PersistentSingleton<UpgradeManager>
{
    [SerializeField] private List<UpgradeConfig> _upgradeConfigs = new();
    private readonly Dictionary<UpgradeType, List<Upgrade>> _upgrades = new();
    public Dictionary<string, Upgrade> upgradeMap = new();
    public event Action OnInitialized;

    private bool _initialized;
    public bool Initialized => _initialized;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Initialize(_upgradeConfigs, DataController.Instance);
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
        _initialized = true;
        Debug.Log("Upgrade Manager Initialized");
        OnInitialized?.Invoke();
    }

    private void SaveUpgrade(string id, BigDouble level, DataController data)
    {
        data.CurrentGameData.upgradeLevels[id] = level;
    }

    public IEnumerable<Upgrade> GetUpgrades(UpgradeType type) =>
        _upgrades.TryGetValue(type, out var upgrades) ? upgrades : Enumerable.Empty<Upgrade>();

    public Upgrade GetUpgrade(UpgradeConfig upgradeConfig) =>
        upgradeMap.TryGetValue(upgradeConfig.upgradeName, out var upgrade) ? upgrade : null;

    public BigDouble GetUpgradeLevel(UpgradeConfig upgradeConfig)
    {
        if (upgradeConfig == null)
        {
            Debug.LogWarning("UpgradeConfig is null");
            return BigDouble.Zero;
        }
        return upgradeMap.TryGetValue(upgradeConfig.upgradeName, out var upgrade)
            ? upgrade.CurrentLevel
            : BigDouble.Zero;
    }

    public void ResetUpgradesExceptPrestige()
    {
        foreach (var upgrade in upgradeMap.Values)
        {
            if (!IsPrestigeUpgrade(upgrade.Config))
            {
                upgrade.UpdateLevel(0);
                Debug.Log("Upgrade level reset: " + upgrade.Config.upgradeName + " to 0");
            }
        }
    }

    public void ResetAllUpgrades()
    {
        foreach (var upgrade in upgradeMap.Values)
        {
            upgrade.UpdateLevel(0);
            Debug.Log("Upgrade level reset: " + upgrade.Config.upgradeName + " to 0");
        }
    }

    public HashSet<string> GetPrestigeUpgradeKeys()
    {
        return upgradeMap.Values
            .Where(u => IsPrestigeUpgrade(u.Config))
            .Select(u => u.Config.upgradeName)
            .ToHashSet();
    }

    private bool IsPrestigeUpgrade(UpgradeConfig config)
    {
        return config.upgradeType == UpgradeType.Prestige;
    }
}