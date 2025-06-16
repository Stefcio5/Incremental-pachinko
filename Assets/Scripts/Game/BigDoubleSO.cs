using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BigDoubleSO", menuName = "BigDouble Value/BigDoubleSO")]
public class BigDoubleSO : ScriptableObject
{
    [SerializeField] private BigDouble _baseValue;
    [SerializeField] private BigDouble _finalValue;
    [SerializeField] private List<BigDoubleSO> _modifiers;
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
    public BigDouble FinalValue => _finalValue;


    void OnEnable()
    {
        SubscribeToModifiers();
        RecalculateFinalValue();
    }


    public void AddModifier(BigDoubleSO modifier)
    {
        if (modifier == null || modifier == this || _modifiers.Contains(modifier))
        {
            return;
        }

        _modifiers.Add(modifier);
        modifier.onValueChanged += RecalculateFinalValue;
        RecalculateFinalValue();

    }
    public void RemoveModifier(BigDoubleSO modifier)
    {
        if (modifier == null || !_modifiers.Contains(modifier))
        {
            return;
        }

        _modifiers.Remove(modifier);
        modifier.onValueChanged -= RecalculateFinalValue;
        RecalculateFinalValue();
    }

    public void ClearModifiers()
    {
        UnsubscribeFromModifiers();
        _modifiers.Clear();
        RecalculateFinalValue();
    }

    private void RecalculateFinalValue()
    {
        _finalValue = _baseValue;
        onValueChanged?.Invoke();
        if (_modifiers.Count == 0) return;

        foreach (var modifier in _modifiers)
        {
            if (modifier.FinalValue == 0) continue;
            _finalValue *= modifier.FinalValue;
        }
        onValueChanged?.Invoke();
    }

    public BigDouble GetFinalValueFor(BigDouble baseValue)
    {
        var result = baseValue;
        foreach (var modifier in _modifiers)
            result *= modifier.FinalValue;
        return result;
    }

    void OnDisable()
    {
        UnsubscribeFromModifiers();
    }
    private void SubscribeToModifiers()
    {
        foreach (var modifier in _modifiers)
        {
            if (modifier != null)
            {
                modifier.onValueChanged += RecalculateFinalValue;
            }
        }
    }

    private void UnsubscribeFromModifiers()
    {
        foreach (var modifier in _modifiers)
        {
            if (modifier != null)
            {
                modifier.onValueChanged -= RecalculateFinalValue;
            }
        }
    }
}
