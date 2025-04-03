using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearFormula", menuName = "Upgrades/Formulas/LinearFormula")]
public class LinearFormula : UpgradeFormula
{
    [SerializeField] private BigDouble multiplier = 1;
    public override BigDouble Calculate(BigDouble baseValue, BigDouble level) => baseValue + (level * multiplier);
}


