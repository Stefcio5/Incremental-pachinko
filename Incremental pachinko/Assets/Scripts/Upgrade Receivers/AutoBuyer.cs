using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoBuyer : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private float purchaseInterval = 1f; // Time in seconds between purchases
    private List<Upgrade> upgrades;
    private float timer;

    private void Start()
    {
        upgrades = UpgradeManager.Instance.GetUpgrades(upgradeType).ToList();
        if (upgrades == null || upgrades.Count == 0)
        {
            Debug.LogWarning($"AutoBuyer: No upgrades found for type {upgradeType}");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= purchaseInterval)
        {
            timer = 0f;
            TryPurchaseUpgrades();
            Debug.Log($"AutoBuyer: Attempting to purchase upgrades of type {upgradeType}");
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