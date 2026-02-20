using System;
using BreakInfinity;
using UnityEngine;

[Serializable]
public class UpgradeEffect
{
    [Tooltip("The stat this effect modifies.")]
    public BigDoubleSO Target;

    [Tooltip("Formula used to compute the modifier's contribution at the current upgrade level.")]
    public UpgradeFormula Formula;

    [Tooltip("Base value passed to the formula. Represents the upgrade's own scaling factor, " +
             "independent of the stat's innate starting value set on the BigDoubleSO asset.")]
    public BigDouble BasePower;

    [Tooltip("How the computed value is applied: Additive adds to the stat base before multiplication; " +
             "Multiplicative scales the result after all additive modifiers.")]
    public ModifierType ModifierType;
}
