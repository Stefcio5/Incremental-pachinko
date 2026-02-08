using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class AutoBuyer : UpgradeReceiver
{
    [SerializeField] private UpgradeConfig _upgradeConfig;
    [SerializeField] private UpgradeType _upgradeType;
    private List<Upgrade> _upgrades;
    private float _timer;

    protected override void Start()
    {
        base.Start();
        _upgrades = UpgradeManager.Instance.GetUpgrades(_upgradeType).ToList();
        if (_upgrades == null || _upgrades.Count == 0)
        {
            Debug.LogWarning($"AutoBuyer: No upgrades found for type {_upgradeType}");
        }
    }

    private void Update()
    {
        if (GetUpgradeLevel(_upgradeConfig) > 0)
        {

            _timer += Time.deltaTime;
            if (_timer >= upgradePower.DisplayValue)
            {
                _timer = 0f;
                TryPurchaseUpgrades();
            }
        }
    }

    private void TryPurchaseUpgrades()
    {
        foreach (var upgrade in _upgrades)
        {
            if (upgrade.CanPurchaseWithoutCost())
            {
                PurchaseUpgrade(upgrade);
            }
        }
    }

    private void PurchaseUpgrade(Upgrade upgrade)
    {
        upgrade.PurchaseWithoutCost();
    }

    private BigDouble GetUpgradeLevel(UpgradeConfig config)
    {
        return UpgradeManager.Instance.GetUpgradeLevel(config);
    }

}