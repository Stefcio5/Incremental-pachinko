using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Upgrades/Formulas/ExponentialFormula")]
public class ExponentialFormula : UpgradeFormula
{
    [SerializeField] private BigDouble exponent = 1.15;
    [SerializeField] public BigDouble Exponent { get => exponent; private set => exponent = value; }
    [SerializeField] private bool useLevelOffset = false;
    [SerializeField] private BigDouble levelOffset = 0;

    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        if (useLevelOffset)
        {
            if (level == 0)
            {
                return baseValue * BigDouble.Pow(exponent, level);
            }
            else
            {
                return baseValue * BigDouble.Pow(exponent, level + levelOffset);
            }

        }
        else return baseValue * BigDouble.Pow(exponent, level);
    }
}


