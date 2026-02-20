using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BigDoubleSO", menuName = "BigDouble Value/BigDoubleSO")]
public class BigDoubleSO : ScriptableObject
{
    [SerializeField] private BigDouble _baseValue;
    [SerializeField] private List<BigDoubleSO> _soModifiers = new();
    private readonly List<StatModifier> _runtimeModifiers = new();

    private BigDouble _displayValue;

    public event Action onValueChanged;

    public BigDouble BaseValue
    {
        get => _baseValue;
        set
        {
            _baseValue = value;
            RecalculateFinalValue();
        }
    }

    public BigDouble DisplayValue => _displayValue;

    public BigDouble FinalValue => _displayValue;

    void OnEnable()
    {
        SubscribeToSoModifiers();
        RecalculateFinalValue();
    }

    void OnDisable()
    {
        UnsubscribeFromSoModifiers();
        _runtimeModifiers.Clear();
    }


    public void AddModifier(StatModifier modifier)
    {
        if (modifier is null)
        {
            return;
        }

        _runtimeModifiers.Add(modifier);
        RecalculateFinalValue();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifier is null)
        {
            return;
        }

        _runtimeModifiers.Remove(modifier);
        RecalculateFinalValue();
    }

    public IReadOnlyList<StatModifier> GetActiveModifiers() => _runtimeModifiers;

    /// <summary>
    /// Forces an immediate recalculation of the displayed value.
    /// Call this when a dynamic <see cref="StatModifier"/> delegate result has changed
    /// (e.g. an upgrade level increased) so subscribers receive an updated <see cref="onValueChanged"/> event.
    /// </summary>
    public void Recalculate() => RecalculateFinalValue();

    public BigDouble GetFinalValueFor(BigDouble baseValue)
    {
        var result = baseValue;

        foreach (var mod in _soModifiers)
        {
            if (mod is not null)
            {
                result *= mod.DisplayValue;
            }
        }

        return result;
    }

    /// <summary>
    /// Runs the full calculation pipeline as it would appear if <paramref name="replacedModifier"/>
    /// contributed <paramref name="simulatedValue"/> instead of its current delegate result.
    /// Does not mutate any state — safe to call for UI preview.
    /// </summary>
    /// <param name="replacedModifier">The modifier whose value should be overridden during simulation.</param>
    /// <param name="simulatedValue">The hypothetical value for that modifier (e.g. at a future level).</param>
    public BigDouble SimulateFinalValue(StatModifier replacedModifier, BigDouble simulatedValue)
    {
        var result = _baseValue;

        // Phase 1: additive runtime modifiers
        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Additive)
            {
                result += ReferenceEquals(mod, replacedModifier) ? simulatedValue : mod.GetValue();
            }
        }

        // Phase 2: SO modifier chain (multiplicative, inspector-wired)
        foreach (var soMod in _soModifiers)
        {
            if (soMod is not null && soMod.DisplayValue != 0)
            {
                result *= soMod.DisplayValue;
            }
        }

        // Phase 3: multiplicative runtime modifiers
        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Multiplicative)
            {
                result *= ReferenceEquals(mod, replacedModifier) ? simulatedValue : mod.GetValue();
            }
        }

        // Phase 4: exponential runtime modifiers
        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Exponential)
            {
                result = BigDouble.Pow(result, ReferenceEquals(mod, replacedModifier) ? simulatedValue : mod.GetValue());
            }
        }

        return result;
    }

    /// <summary>
    /// Overload of <see cref="SimulateFinalValue(StatModifier, BigDouble)"/> that accepts multiple
    /// simultaneous substitutions. Use when two or more modifiers from the same upgrade must be
    /// evaluated at a hypothetical future state together (e.g. formula modifier + step multiplier).
    /// </summary>
    /// <param name="substitutions">Map of modifier → hypothetical value. Modifiers absent from the
    /// map are evaluated normally via their delegate.</param>
    public BigDouble SimulateFinalValue(Dictionary<StatModifier, BigDouble> substitutions)
    {
        BigDouble Resolve(StatModifier mod) =>
            substitutions.TryGetValue(mod, out var v) ? v : mod.GetValue();

        var result = _baseValue;

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Additive)
                result += Resolve(mod);
        }

        foreach (var soMod in _soModifiers)
        {
            if (soMod is not null && soMod.DisplayValue != 0)
                result *= soMod.DisplayValue;
        }

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Multiplicative)
                result *= Resolve(mod);
        }

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Exponential)
                result = BigDouble.Pow(result, Resolve(mod));
        }

        return result;
    }

    private void RecalculateFinalValue()
    {
        var result = _baseValue;

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Additive)
            {
                result += mod.GetValue();
            }
        }

        foreach (var soMod in _soModifiers)
        {
            if (soMod is not null && soMod.DisplayValue != 0)
            {
                result *= soMod.DisplayValue;
            }
        }

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Multiplicative)
            {
                result *= mod.GetValue();
            }
        }

        foreach (var mod in _runtimeModifiers)
        {
            if (mod.Type == ModifierType.Exponential)
            {
                result = BigDouble.Pow(result, mod.GetValue());
            }
        }

        _displayValue = result;
        onValueChanged?.Invoke();
    }

    private void SubscribeToSoModifiers()
    {
        foreach (var mod in _soModifiers)
        {
            if (mod is not null)
            {
                mod.onValueChanged += RecalculateFinalValue;
            }
        }
    }

    private void UnsubscribeFromSoModifiers()
    {
        foreach (var mod in _soModifiers)
        {
            if (mod is not null)
            {
                mod.onValueChanged -= RecalculateFinalValue;
            }
        }
    }
}
