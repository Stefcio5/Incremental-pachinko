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

    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        this.config = config;
        CurrentLevel = initialLevel;
        CalculateBaseValue();
        CurrentPower = config.upgradePower;
    }

    public bool CanBuy(BigDouble availablePoints)
    {
        return !IsMaxLevelReached && availablePoints >= CurrentCost;
    }

    public void UpdateLevel(BigDouble newlevel)
    {
        CurrentLevel = newlevel;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    public void CalculateBaseValue()
    {
        if (config.upgradePower != null)
        {
            config.upgradePower.BaseValue = _currentPower;
            Debug.Log($"Upgrade {config.upgradeName} base value {config.upgradePower.BaseValue} final value {config.upgradePower.FinalValue}");
        }
    }

    public void LevelUp()
    {
        if (CanBuy(CurrentCost))
        {
            CurrentLevel++;
            OnLevelChanged?.Invoke(this);
            CalculateBaseValue();
        }
    }

    private bool IsMaxLevelReached => config.hasMaxLevel && CurrentLevel >= config.maxLevel;
}
