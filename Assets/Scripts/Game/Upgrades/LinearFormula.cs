using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearFormula", menuName = "Upgrades/Formulas/LinearFormula")]
public class LinearFormula : UpgradeFormula
{
    [SerializeField] private BigDouble _multiplier = 1;
    [SerializeField] private bool _isLinear;
    [SerializeField] private bool _isAdditive;
    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        if (_isLinear)
            return baseValue * level * _multiplier;

        else if (_isAdditive)
            return baseValue + (level * _multiplier);
        else
            return baseValue * (1 + (level * _multiplier));
    }
}


