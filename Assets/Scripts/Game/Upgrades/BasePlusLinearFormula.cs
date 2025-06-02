using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BasePlusLinearFormula", menuName = "Upgrades/Formulas/BasePlusLinearFormula")]
public class BasePlusLinearFormula : UpgradeFormula
{
    [SerializeField] private BigDouble _multiplier = 1;
    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        return baseValue * (1 + (level * _multiplier));
    }
}


