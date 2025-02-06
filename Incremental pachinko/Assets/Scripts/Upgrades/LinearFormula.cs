using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "LinearFormula", menuName = "Upgrades/Formulas/LinearFormula")]
public class LinearFormula : ScriptableObject, IUpgradeFormula
{
    [SerializeField] private BigDouble multiplier = 1;
    public BigDouble Calculate(BigDouble baseValue, BigDouble level) => baseValue + (level * multiplier);
}


