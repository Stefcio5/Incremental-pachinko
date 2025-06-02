using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "AdditiveLinearFormula", menuName = "Upgrades/Formulas/AdditiveLinearFormula")]
public class AdditiveLinearFormula : UpgradeFormula
{
    [SerializeField] private BigDouble _multiplier = 1;
    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        return baseValue + (level * _multiplier);
    }
}


