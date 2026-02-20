using System;
using System.Collections.Generic;
using BreakInfinity;

public class Upgrade : IDisposable
{
    // --- FIELDS ---
    private BuyAmountStrategy _buyAmountStrategy;
    private IUpgradePurchaseStrategy _purchaseStrategy;

    private readonly List<(BigDoubleSO target, StatModifier modifier)> _effectModifiers = new();

    // --- PROPERTIES ---
    public UpgradeConfig Config { get; }
    public BigDouble CurrentLevel { get; private set; }
    public BigDouble PurchaseLevel => CurrentLevel - Config.StartingLevel;
    public BuyAmountStrategy BuyAmountStrategy => _buyAmountStrategy;
    public BigDouble CurrentCost => _buyAmountStrategy.GetCost(this);

    /// <summary>
    /// The primary stat targeted by this upgrade (first effect).
    /// Used by UI to subscribe to value-changed events and display the current stat value.
    /// Returns null if no effects are defined.
    /// </summary>
    public BigDoubleSO PrimaryTarget => Config.Effects is { Count: > 0 } ? Config.Effects[0].Target : null;

    private bool IsMaxLevelReached => Config.hasMaxLevel && CurrentLevel >= Config.maxLevel;

    // --- EVENTS ---
    public event Action<Upgrade> OnLevelChanged;

    // --- CONSTRUCTOR ---
    public Upgrade(UpgradeConfig config, BigDouble initialLevel)
    {
        Config = config;
        CurrentLevel = initialLevel;
        _buyAmountStrategy = config.buyAmountStrategy;
        _purchaseStrategy = UpgradePurchaseStrategyFactory.Create(config.upgradeType);

        RegisterEffectModifiers();
        RecalculateAllTargets();

        BuyAmountController.OnBuyAmountStrategyChanged += SetBuyAmountStrategy;
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
        RecalculateAllTargets();
        OnLevelChanged?.Invoke(this);
    }

    /// <summary>
    /// Returns the step multiplier applied to formula output when the step-multiplier feature is enabled.
    /// Computed from how many full intervals fit into <paramref name="level"/>.
    /// </summary>
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

    /// <summary>
    /// Returns the simulated stat value for the primary effect after the next purchase.
    /// Runs the full <see cref="BigDoubleSO"/> pipeline (base + additive + SO chain + multiplicative)
    /// with this upgrade's modifiers substituted at the future level, so the preview is always accurate
    /// regardless of buy-amount strategy or other active modifiers (PowerUps, Cards, etc.).
    /// Both the formula modifier and the step-multiplier modifier (if present) are evaluated at
    /// the future level simultaneously so they interact correctly.
    /// </summary>
    public BigDouble GetNextPower()
    {
        if (Config.Effects is not { Count: > 0 } || _effectModifiers.Count == 0) return 0;

        var e = Config.Effects[0];
        var (target, formulaModifier) = _effectModifiers[0];
        if (target is null || e.Formula is null) return 0;

        var buyAmount = _buyAmountStrategy.GetBuyAmount(this);
        var nextLevel = CurrentLevel + buyAmount;

        var substitutions = new Dictionary<StatModifier, BigDouble>
        {
            [formulaModifier] = e.Formula.Calculate(e.BasePower, nextLevel)
        };

        // When step multiplier is active it is stored as the modifier immediately after the formula
        // modifier for the same effect (index 1 for a single-effect upgrade).
        if (Config.useStepMultiplier && _effectModifiers.Count > 1 && _effectModifiers[1].target == target)
        {
            substitutions[_effectModifiers[1].modifier] = GetStepMultiplier(nextLevel);
        }

        return target.SimulateFinalValue(substitutions);
    }

    /// <summary>
    /// Removes all registered stat modifiers from their targets and unsubscribes global events.
    /// Must be called when the upgrade is no longer in use to avoid dangling modifier references.
    /// </summary>
    public void Dispose()
    {
        BuyAmountController.OnBuyAmountStrategyChanged -= SetBuyAmountStrategy;

        foreach (var (target, modifier) in _effectModifiers)
        {
            target.RemoveModifier(modifier);
        }

        _effectModifiers.Clear();
    }

    public void OnDestroy()
    {
        Dispose();
    }

    // --- PRIVATE METHODS ---

    /// <summary>
    /// Creates one dynamic <see cref="StatModifier"/> per effect and registers it with the target stat.
    /// The modifier delegate captures the effect and this upgrade by reference so it always reflects
    /// the current level when <see cref="BigDoubleSO.RecalculateFinalValue"/> is invoked.
    /// When <see cref="UpgradeConfig.useStepMultiplier"/> is enabled, a second <em>multiplicative</em>
    /// modifier is registered on the same target so the step multiplier scales the full stat value
    /// (base + all additive modifiers) rather than just the formula output.
    /// </summary>
    private void RegisterEffectModifiers()
    {
        if (Config.Effects is null) return;

        foreach (var effect in Config.Effects)
        {
            if (effect?.Target is null || effect.Formula is null) continue;

            // Capture loop variable explicitly to avoid closure-over-loop-variable bug.
            var capturedEffect = effect;

            // Formula modifier — pure formula output, no step multiplier baked in.
            var formulaModifier = new StatModifier(
                capturedEffect.ModifierType,
                ModifierSource.Upgrade,
                Config.upgradeName,
                () => capturedEffect.Formula.Calculate(capturedEffect.BasePower, CurrentLevel));

            capturedEffect.Target.AddModifier(formulaModifier);
            _effectModifiers.Add((capturedEffect.Target, formulaModifier));

            // Step multiplier modifier — registered as Multiplicative so it scales the full stat
            // (base value + all additive contributions) rather than only the formula output.
            if (Config.useStepMultiplier)
            {
                var stepModifier = new StatModifier(
                    ModifierType.Multiplicative,
                    ModifierSource.Upgrade,
                    $"{Config.upgradeName} (step)",
                    () => GetStepMultiplier(CurrentLevel));

                capturedEffect.Target.AddModifier(stepModifier);
                _effectModifiers.Add((capturedEffect.Target, stepModifier));
            }
        }
    }

    private void RecalculateAllTargets()
    {
        foreach (var (target, _) in _effectModifiers)
        {
            target.Recalculate();
        }
    }

    private void IncreaseLevel(BigDouble amount)
    {
        CurrentLevel += amount;
        OnLevelChanged?.Invoke(this);
        RecalculateAllTargets();
    }
}
