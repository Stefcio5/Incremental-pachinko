using System;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Upgrades/Formulas/ExponentialFormula")]
public class ExponentialFormula : UpgradeFormula
{
    private enum Mode { PowerLevel, LevelPower };

    [SerializeField] private Mode exponentMode;
    [SerializeField] private BigDouble _exponent = 1.15;
    [SerializeField] public BigDouble Exponent { get => _exponent; private set => _exponent = value; }
    [SerializeField] private bool _useLevelOffset = false;
    [SerializeField] private BigDouble _levelOffset = 0;

    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        var adjustedLevel = _useLevelOffset
            ? level + _levelOffset
            : level;

        return exponentMode switch
        {
            Mode.PowerLevel => baseValue * BigDouble.Pow(_exponent, adjustedLevel),
            Mode.LevelPower => baseValue * BigDouble.Pow(adjustedLevel, _exponent),
            _ => baseValue
        };
    }
}


