using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BreakInfinity;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UpgradeManager : PersistentSingleton<UpgradeManager>, IGameSystem
{
    [SerializeField] private List<UpgradeConfig> _upgradeConfigs = new();

    private readonly Dictionary<UpgradeType, List<Upgrade>> _upgrades = new();
    public Dictionary<string, Upgrade> upgradeMap = new();

    public event Action OnInitialized;
    public event Action OnSystemInitialized;

    private bool _initialized;
    public bool Initialized => _initialized;

    public string SystemName => "UpgradeManager";
    public bool IsInitialized => _initialized;

    protected override void Awake()
    {
        base.Awake();
    }


    public async UniTask InitializeAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            Debug.LogWarning($"[{SystemName}] Already initialized");
            return;
        }

        progress?.Report(0f);

        try
        {
            // Wait for DataController to be initialized
            await UniTask.WaitUntil(() => DataController.Instance != null && DataController.Instance.IsInitialized,
                PlayerLoopTiming.Update, cancellationToken);
            progress?.Report(0.5f);

            // Initialize with configurations and data
            Initialize(_upgradeConfigs, DataController.Instance);
            progress?.Report(1f);

            OnSystemInitialized?.Invoke();
            Debug.Log($"[{SystemName}] Async initialization complete");
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[{SystemName}] Initialization cancelled");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[{SystemName}] Initialization failed: {e.Message}");
            throw;
        }
    }

    private void Initialize(IEnumerable<UpgradeConfig> configs, DataController data)
    {
        if (configs == null || data == null)
        {
            throw new ArgumentNullException("configs or data is null");
        }

        foreach (var upgrade in upgradeMap.Values)
        {
            upgrade.Dispose();
        }

        _upgrades.Clear();
        upgradeMap.Clear();

        foreach (var config in configs)
        {
            if (config == null) continue;

            BigDouble initialLevel = data.CurrentGameData.upgradeLevels.GetValueOrDefault(config.upgradeName, config.StartingLevel);
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
        Debug.Log($"[{SystemName}] Initialized with {upgradeMap.Count} upgrades");
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
                upgrade.UpdateLevel(upgrade.Config.StartingLevel);
                Debug.Log("Upgrade level reset: " + upgrade.Config.upgradeName + " to " + upgrade.Config.StartingLevel);
            }
        }
    }

    public void ResetAllUpgrades()
    {
        foreach (var upgrade in upgradeMap.Values)
        {
            upgrade.UpdateLevel(upgrade.Config.StartingLevel);
            Debug.Log("Upgrade level reset: " + upgrade.Config.upgradeName + " to " + upgrade.Config.StartingLevel);
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