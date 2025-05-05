using System;
using BreakInfinity;
using UnityEngine;

public class Upgrade
{
    public readonly UpgradeConfig config;
    public BigDouble CurrentLevel { get; set; }
    public BigDouble CurrentCost => config.costFormula.Calculate(config.baseCost, CurrentLevel);
    private BigDouble _currentPower => config.powerFormula.Calculate(config.basePower, CurrentLevel);
    public BigDoubleSO CurrentPower;
    public event Action<Upgrade> OnLevelChanged;
    private IUpgradePurchaseStrategy _purchaseStrategy;

    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        this.config = config;
        CurrentLevel = initialLevel;
        CurrentPower = config.upgradePower;
        CalculateBaseValue();
        _purchaseStrategy = UpgradePurchaseStrategyFactory.Create(config.upgradeType);
    }

    public bool CanPurchase()
    {
        return !IsMaxLevelReached && _purchaseStrategy.CanPurchase(CurrentCost);
    }

    public void UpdateLevel(BigDouble newlevel)
    {
        CurrentLevel = newlevel;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    public void CalculateBaseValue()
    {
        if (CurrentPower != null)
        {
            CurrentPower.BaseValue = _currentPower;
            Debug.Log($"Upgrade {config.upgradeName} base value {config.upgradePower.BaseValue} final value {config.upgradePower.FinalValue}");
        }
    }

    public void Purchase()
    {
        if (!CanPurchase()) return;

        _purchaseStrategy.PurchaseUpgrade(CurrentCost);
        CurrentLevel++;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    public void PurchaseWithoutCost()
    {
        if (!CanPurchase()) return;

        CurrentLevel++;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    private bool IsMaxLevelReached => config.hasMaxLevel && CurrentLevel >= config.maxLevel;
}
