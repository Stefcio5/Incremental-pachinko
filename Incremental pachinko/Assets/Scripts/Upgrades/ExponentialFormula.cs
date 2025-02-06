using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Upgrades/Formulas/ExponentialFormula")]
public class ExponentialFormula : ScriptableObject, IUpgradeFormula
{
    [SerializeField] private BigDouble exponent = 1.15;

    public BigDouble Calculate(BigDouble baseValue, BigDouble level) => baseValue * BigDouble.Pow(exponent, level);
}


