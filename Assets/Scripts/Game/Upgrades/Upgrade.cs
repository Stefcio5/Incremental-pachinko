using System;
using BreakInfinity;
using UnityEngine;

public class Upgrade : IDisposable
{
    public UpgradeConfig Config { get; }
    public BigDouble CurrentLevel { get; set; }
    private BuyAmountStrategy _buyAmountStrategy;
    public BuyAmountStrategy BuyAmountStrategy => _buyAmountStrategy;
    public BigDouble CurrentCost => _buyAmountStrategy.GetCost(this);
    public BigDoubleSO CurrentPower;
    public event Action<Upgrade> OnLevelChanged;
    private IUpgradePurchaseStrategy _purchaseStrategy;

    private BigDouble _currentPower => Config.powerFormula.Calculate(Config.basePower, CurrentLevel);

    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        Config = config;
        CurrentLevel = initialLevel;
        CurrentPower = config.upgradePower;
        _buyAmountStrategy = config.buyAmountStrategy;
        BuyAmountController.OnBuyAmountStrategyChanged += SetBuyAmountStrategy;
        CalculateBaseValue();
        _purchaseStrategy = UpgradePurchaseStrategyFactory.Create(config.upgradeType);
    }

    public void SetBuyAmountStrategy(BuyAmountStrategy buyAmountStrategy)
    {
        _buyAmountStrategy = buyAmountStrategy;
    }

    public bool CanPurchase()
    {
        return !IsMaxLevelReached && _purchaseStrategy.CanPurchase(CurrentCost);
    }
    private bool CanPurchaseWithoutCost()
    {
        return !IsMaxLevelReached && _purchaseStrategy.CanPurchase(Config.costFormula.Calculate(Config.baseCost, CurrentLevel));
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
            Debug.Log($"Upgrade {Config.upgradeName} base value {Config.upgradePower.BaseValue} final value {Config.upgradePower.FinalValue}");
        }
    }

    public void Purchase()
    {
        if (!CanPurchase()) return;

        _purchaseStrategy.PurchaseUpgrade(CurrentCost);
        CurrentLevel += _buyAmountStrategy.GetBuyAmount(this);
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    public void PurchaseWithoutCost()
    {
        if (!CanPurchaseWithoutCost()) return;

        CurrentLevel++;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue();
    }

    private bool IsMaxLevelReached => Config.hasMaxLevel && CurrentLevel >= Config.maxLevel;

    public void OnDestroy()
    {
        BuyAmountController.OnBuyAmountStrategyChanged -= SetBuyAmountStrategy;
    }

    public void Dispose()
    {
        OnDestroy();
    }
}
