using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Upgrades/Formulas/ExponentialFormula")]
public class ExponentialFormula : UpgradeFormula
{
    [SerializeField] private BigDouble _exponent = 1.15;
    [SerializeField] public BigDouble Exponent { get => _exponent; private set => _exponent = value; }
    [SerializeField] private bool _useLevelOffset = false;
    [SerializeField] private BigDouble _levelOffset = 0;

    public override BigDouble Calculate(BigDouble baseValue, BigDouble level)
    {
        if (_useLevelOffset)
        {
            if (level == 0)
            {
                return baseValue * BigDouble.Pow(_exponent, level);
            }
            else
            {
                return baseValue * BigDouble.Pow(_exponent, level + _levelOffset);
            }

        }
        else return baseValue * BigDouble.Pow(_exponent, level);
    }
}


