using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearFormula", menuName = "Upgrades/Formulas/LinearFormula")]
public class LinearFormula : UpgradeFormula
{
    [SerializeField] private BigDouble multiplier = 1;
    [SerializeField] private bool isAdditive;
    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        if (isAdditive)
            return baseValue + (level * multiplier);
        else          
            return baseValue * (1 + (level * multiplier));
    }
}


