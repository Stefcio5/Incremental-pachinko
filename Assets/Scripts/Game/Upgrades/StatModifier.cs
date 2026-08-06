using System;
using BreakInfinity;

[Serializable]
public class StatModifier
{
    public readonly ModifierType Type;

    public readonly ModifierSource Source;

    public readonly string Label;

    private readonly BigDouble _staticValue;
    private readonly Func<BigDouble> _dynamicValue;

    public StatModifier(ModifierType type, ModifierSource source, string label, BigDouble value)
    {
        Type = type;
        Source = source;
        Label = label;
        _staticValue = value;
    }

    /// <summary>
    /// Use this overload when the modifier value must reflect a live game value,
    /// e.g. an upgrade whose contribution scales with its current level.
    /// </summary>
    /// <param name="dynamicValue">Delegate evaluated each time GetValue is called.</param>
    public StatModifier(ModifierType type, ModifierSource source, string label, Func<BigDouble> dynamicValue)
    {
        Type = type;
        Source = source;
        Label = label;
        _dynamicValue = dynamicValue;
    }

    public BigDouble GetValue() => _dynamicValue?.Invoke() ?? _staticValue;
}

public enum ModifierType
{
    Additive,
    Multiplicative,
    Exponential,
}

public enum ModifierSource
{
    Upgrade,
    PowerUp,
    Card,
    Skill,
    Achievement,
}
