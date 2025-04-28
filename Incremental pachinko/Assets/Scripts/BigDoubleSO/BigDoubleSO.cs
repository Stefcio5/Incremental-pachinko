using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BigDoubleSO", menuName = "BigDouble Value/BigDoubleSO")]
public class BigDoubleSO : ScriptableObject
{
    [SerializeField] private BigDouble baseValue;
    [SerializeField] private BigDouble cachedFinalValue;
    [SerializeField, HideInInspector] private bool isDirty = true;

    [SerializeField] private List<BigDoubleSO> modifiers = new List<BigDoubleSO>();
    [SerializeField] public event Action onValueChanged;

    public BigDouble BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            MarkDirty();
        }
    }
    public BigDouble FinalValue
    {
        get
        {
            if (isDirty)
            {
                RecalculateFinalValue();
            }
            return cachedFinalValue;
        }
    }
    public void AddModifier(BigDoubleSO modifier)
    {
        if (!modifiers.Contains(modifier) && modifier != this)
        {
            modifiers.Add(modifier);
            MarkDirty();
        }
    }
    public void RemoveModifier(BigDoubleSO modifier)
    {
        if (modifiers.Contains(modifier))
        {
            modifiers.Remove(modifier);
            MarkDirty();
        }
    }

    public void ClearModifiers()
    {
        modifiers.Clear();
        MarkDirty();
    }

    private void MarkDirty()
    {
        isDirty = true;
        onValueChanged?.Invoke();
    }

    private void RecalculateFinalValue()
    {
        cachedFinalValue = baseValue;
        if (modifiers.Count == 0)
        {
            isDirty = false;
            return;
        }
        foreach (var modifier in modifiers)
        {
            cachedFinalValue *= modifier.FinalValue;
        }
        isDirty = false;
    }
}
