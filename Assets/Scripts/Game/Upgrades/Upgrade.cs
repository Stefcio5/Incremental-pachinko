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
    public BigDouble PurchaseLevel => CurrentLevel - Config.StartingLevel;
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
        UpdateCurrentPower();
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
        var cost = Config.costFormula.Calculate(Config.baseCost, PurchaseLevel);
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
        UpdateCurrentPower();
        OnLevelChanged?.Invoke(this);
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

    public BigDouble GetNextPower()
    {
        var buyAmount = _buyAmountStrategy.GetBuyAmount(this);
        var nextBase = CalculatePowerAtLevel(CurrentLevel + buyAmount);
        return CurrentPower.GetFinalValueFor(nextBase);
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

    private BigDouble CalculatePowerAtLevel(BigDouble level)
    {
        var basePower = Config.powerFormula.Calculate(Config.basePower, level);
        return basePower * GetStepMultiplier(level);
    }

    private void UpdateCurrentPower()
    {
        CurrentPower.BaseValue = CalculatePowerAtLevel(CurrentLevel);
    }

    private void IncreaseLevel(BigDouble amount)
    {
        CurrentLevel += amount;
        OnLevelChanged?.Invoke(this);
        UpdateCurrentPower();
    }
}
