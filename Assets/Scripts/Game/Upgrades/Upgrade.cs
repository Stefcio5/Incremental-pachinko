using System;
using BreakInfinity;

public class Upgrade : IDisposable
{
    // --- FIELDS ---
    private BuyAmountStrategy _buyAmountStrategy;
    private IUpgradePurchaseStrategy _purchaseStrategy;

    // --- PROPERTIES ---
    public UpgradeConfig Config { get; }
    public BigDouble CurrentLevel { get; private set; }
    public BuyAmountStrategy BuyAmountStrategy => _buyAmountStrategy;
    public BigDouble CurrentCost => _buyAmountStrategy.GetCost(this);
    public BigDoubleSO CurrentPower { get; private set; }
    private bool IsMaxLevelReached => Config.hasMaxLevel && CurrentLevel >= Config.maxLevel;

    // --- EVENTS ---
    public event Action<Upgrade> OnLevelChanged;

    // --- CONSTRUCTOR ---
    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        Config = config;
        CurrentLevel = initialLevel;
        CurrentPower = config.upgradePower;
        _buyAmountStrategy = config.buyAmountStrategy;
        _purchaseStrategy = UpgradePurchaseStrategyFactory.Create(config.upgradeType);

        BuyAmountController.OnBuyAmountStrategyChanged += SetBuyAmountStrategy;
        CalculateBaseValue(CurrentLevel);
    }

    // --- PUBLIC METHODS ---

    public void SetBuyAmountStrategy(BuyAmountStrategy buyAmountStrategy)
    {
        _buyAmountStrategy = buyAmountStrategy;
    }

    public bool CanPurchase()
    {
        return !IsMaxLevelReached && _purchaseStrategy.CanPurchase(CurrentCost);
    }

    public bool CanPurchaseWithoutCost()
    {
        var cost = Config.costFormula.Calculate(Config.baseCost, CurrentLevel);
        return !IsMaxLevelReached && _purchaseStrategy.CanPurchase(cost);
    }

    public void Purchase()
    {
        if (!CanPurchase()) return;

        _purchaseStrategy.PurchaseUpgrade(CurrentCost);
        IncreaseLevel(_buyAmountStrategy.GetBuyAmount(this));
    }

    public void PurchaseWithoutCost()
    {
        if (!CanPurchaseWithoutCost()) return;

        IncreaseLevel(1);
    }

    public void UpdateLevel(BigDouble newLevel)
    {
        CurrentLevel = newLevel;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue(CurrentLevel);
    }

    public BigDouble GetNextPower()
    {
        var nextLevel = CurrentLevel + _buyAmountStrategy.GetBuyAmount(this);
        return GetPowerAtLevel(nextLevel) * GetStepMultiplier(nextLevel);
    }

    public BigDouble GetStepMultiplier(BigDouble level)
    {
        if (!Config.useStepMultiplier) return 1;

        int steps = (int)(level / Config.multiplierInterval);
        return BigDouble.Pow(Config.multiplierBase, steps);
    }

    public float GetCurrentStepValue()
    {
        return (float)CurrentLevel % (float)Config.multiplierInterval / (float)Config.multiplierInterval;
    }

    public void Dispose()
    {
        BuyAmountController.OnBuyAmountStrategyChanged -= SetBuyAmountStrategy;
    }

    public void OnDestroy()
    {
        Dispose();
    }

    // --- PRIVATE METHODS ---

    private BigDouble GetPowerAtLevel(BigDouble level)
    {
        return Config.powerFormula.Calculate(Config.basePower, level);
    }

    private void CalculateBaseValue(BigDouble level)
    {
        if (CurrentPower != null)
        {
            CurrentPower.BaseValue = GetPowerAtLevel(level) * GetStepMultiplier(level);
        }
    }

    private void IncreaseLevel(BigDouble amount)
    {
        CurrentLevel += amount;
        OnLevelChanged?.Invoke(this);
        CalculateBaseValue(CurrentLevel);
    }
}
