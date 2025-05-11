using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoBuyer : UpgradeReceiver
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private float purchaseInterval = 1f; // Time in seconds between purchases
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
        if (!(upgradePower.FinalValue >= 1f))
        {

            timer += Time.deltaTime;
            if (timer >= upgradePower.FinalValue)
            {
                timer = 0f;
                TryPurchaseUpgrades();
                Debug.Log($"AutoBuyer: Attempting to purchase upgrades of type {upgradeType}");
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


}