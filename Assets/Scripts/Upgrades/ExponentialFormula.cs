using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Upgrades/Formulas/ExponentialFormula")]
public class ExponentialFormula : UpgradeFormula
{
    [SerializeField] private BigDouble exponent = 1.15;

    public override BigDouble Calculate(BigDouble baseValue, BigDouble level) => baseValue * BigDouble.Pow(exponent, level);
}


