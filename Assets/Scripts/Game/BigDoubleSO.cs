using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BigDoubleSO", menuName = "BigDouble Value/BigDoubleSO")]
public class BigDoubleSO : ScriptableObject
{
    [SerializeField] private BigDouble _baseValue;
    [SerializeField] private BigDouble _displayValue;
    [SerializeField] private List<BigDoubleSO> _modifiers;
    [SerializeField] private List<PowerUpConfig> _powerUpConfigs = new();
    private BigDouble _powerUpModifier = 1;
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
    public BigDouble FinalValue => _displayValue * _powerUpModifier;


    void OnEnable()
    {
        SubscribeToModifiers();
        RecalculateFinalValue();
    }

    public void AddPowerUp(PowerUpConfig config)
    {
        if (config == null) return;
        _powerUpConfigs.Add(config);
        ApplyPowerUps();
    }
    private void ApplyPowerUps()
    {
        _powerUpModifier = 1;
        foreach (var powerUp in _powerUpConfigs)
        {
            _powerUpModifier *= powerUp.Multiplier;
        }
        onValueChanged?.Invoke();
    }

    public void RemovePowerUp(PowerUpConfig config)
    {
        if (config == null) return;
        _powerUpConfigs.Remove(config);
        ApplyPowerUps();
    }
    public bool HasPowerUp(PowerUpConfig config)
    {
        return _powerUpConfigs.Contains(config);
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
        _displayValue = _baseValue;
        onValueChanged?.Invoke();
        if (_modifiers.Count == 0) return;

        foreach (var modifier in _modifiers)
        {
            if (modifier.DisplayValue == 0) continue;
            _displayValue *= modifier.DisplayValue;
        }
        onValueChanged?.Invoke();
    }

    public BigDouble GetFinalValueFor(BigDouble baseValue)
    {
        var result = baseValue;
        foreach (var modifier in _modifiers)
            result *= modifier.DisplayValue;
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
