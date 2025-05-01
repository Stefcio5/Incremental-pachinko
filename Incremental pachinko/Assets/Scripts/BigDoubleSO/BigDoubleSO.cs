using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BigDoubleSO", menuName = "BigDouble Value/BigDoubleSO")]
public class BigDoubleSO : ScriptableObject
{
    [SerializeField] private BigDouble baseValue;
    [SerializeField] private BigDouble finalValue;
    [SerializeField] private List<BigDoubleSO> modifiers;
    [SerializeField] public event Action onValueChanged;

    public BigDouble BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            RecalculateFinalValue();
        }
    }
    public BigDouble FinalValue => finalValue;


    void OnEnable()
    {
        SubscribeToModifiers();
        RecalculateFinalValue();
    }


    public void AddModifier(BigDoubleSO modifier)
    {
        if (modifier == null || modifier == this || modifiers.Contains(modifier))
        {
            return;
        }

        modifiers.Add(modifier);
        modifier.onValueChanged += RecalculateFinalValue;
        RecalculateFinalValue();

    }
    public void RemoveModifier(BigDoubleSO modifier)
    {
        if (modifier == null || !modifiers.Contains(modifier))
        {
            return;
        }

        modifiers.Remove(modifier);
        modifier.onValueChanged -= RecalculateFinalValue;
        RecalculateFinalValue();
    }

    public void ClearModifiers()
    {
        UnsubscribeFromModifiers();
        modifiers.Clear();
        RecalculateFinalValue();
    }

    private void RecalculateFinalValue()
    {
        finalValue = baseValue;
        onValueChanged?.Invoke();
        if (modifiers.Count == 0) return;

        foreach (var modifier in modifiers)
        {
            finalValue *= modifier.FinalValue;
        }
        onValueChanged?.Invoke();
    }

    void OnDisable()
    {
        UnsubscribeFromModifiers();
    }
    private void SubscribeToModifiers()
    {
        foreach (var modifier in modifiers)
        {
            if (modifier != null)
            {
                modifier.onValueChanged += RecalculateFinalValue;
            }
        }
    }

    private void UnsubscribeFromModifiers()
    {
        foreach (var modifier in modifiers)
        {
            if (modifier != null)
            {
                modifier.onValueChanged -= RecalculateFinalValue;
            }
        }
    }
}
