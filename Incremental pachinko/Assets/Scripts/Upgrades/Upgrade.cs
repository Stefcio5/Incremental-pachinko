using System;
using BreakInfinity;
using UnityEngine;

public class Upgrade
{
    public readonly UpgradeConfig config;
    public BigDouble CurrentLevel { get; set; }
    public BigDouble CurrentCost => config.costFormula.Calculate(config.baseCost, CurrentLevel);
    public BigDouble CurrentPower => config.powerFormula.Calculate(config.basePower, CurrentLevel);
    public event Action<Upgrade> OnLevelChanged;

    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        this.config = config;
        CurrentLevel = initialLevel;
    }

    public bool CanBuy(BigDouble availablePoints)
    {
        return !IsMaxLevelReached && availablePoints >= CurrentCost;
    }

    public void UpdateLevel(BigDouble newlevel)
    {
        CurrentLevel = newlevel;
        OnLevelChanged?.Invoke(this);
    }

    public void LevelUp()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke(this);
    }

    private bool IsMaxLevelReached => config.hasMaxLevel && CurrentLevel >= config.maxLevel;
}
