using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class AutoBuyer : UpgradeReceiver
{
    [SerializeField] private UpgradeConfig _upgradeConfig;
    [SerializeField] private UpgradeType _upgradeType;

    private List<Upgrade> _upgrades = new();
    private float _timer;
    private bool _isReady;

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        InitializeUpgrades();
        _isReady = true;
    }

    private void InitializeUpgrades()
    {
        _upgrades = UpgradeManager.Instance.GetUpgrades(_upgradeType).ToList();

        if (_upgrades is null || _upgrades.Count == 0)
        {
            Debug.LogWarning($"[{nameof(AutoBuyer)}] No upgrades found for type {_upgradeType}");
        }
    }

    private void Update()
    {
        if (!_isReady || _upgrades is null || _upgrades.Count == 0)
        {
            return;
        }

        if (GetUpgradeLevel(_upgradeConfig) <= 0)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= upgradePower.DisplayValue)
        {
            _timer = 0f;
            TryPurchaseUpgrades();
        }
    }

    private void TryPurchaseUpgrades()
    {
        foreach (var upgrade in _upgrades)
        {
            if (upgrade.CanPurchaseWithoutCost())
            {
                upgrade.PurchaseWithoutCost();
            }
        }
    }

    private BigDouble GetUpgradeLevel(UpgradeConfig config)
    {
        return UpgradeManager.Instance.GetUpgradeLevel(config);
    }
}