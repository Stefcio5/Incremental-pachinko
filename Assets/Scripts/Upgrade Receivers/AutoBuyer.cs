using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class AutoBuyer : UpgradeReceiver
{
    [SerializeField] private UpgradeConfig upgradeConfig;
    [SerializeField] private UpgradeType upgradeType;
    private List<Upgrade> upgrades;
    private float timer;

    protected override void Start()
    {
        base.Start();
        upgrades = UpgradeManager.Instance.GetUpgrades(upgradeType).ToList();
        if (upgrades == null || upgrades.Count == 0)
        {
            Debug.LogWarning($"AutoBuyer: No upgrades found for type {upgradeType}");
        }
    }

    private void Update()
    {
        if (GetUpgradeLevel(upgradeConfig) > 0)
        {

            timer += Time.deltaTime;
            if (timer >= upgradePower.FinalValue)
            {
                timer = 0f;
                TryPurchaseUpgrades();
            }
        }
    }

    private void TryPurchaseUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade.CanPurchase())
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