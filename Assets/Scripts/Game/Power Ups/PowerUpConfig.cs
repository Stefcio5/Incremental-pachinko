using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpConfig", menuName = "Scriptable Objects/PowerUpConfig")]
public class PowerUpConfig : ScriptableObject
{
    public string Name;
    public float Duration;

    [Header("Effects")]
    [Tooltip("Each effect targets one stat with its own modifier type and value.")]
    public List<PowerUpEffect> Effects = new();
}

[Serializable]
public class PowerUpEffect
{
    [Tooltip("The stat this effect modifies.")]
    public BigDoubleSO Target;

    [Tooltip("Additive: flat bonus added before multiplication. Multiplicative: multiplier applied after additive bonuses.")]
    public ModifierType ModifierType;

    public BigDouble Value;
}
