using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyXStrategy", menuName = "Upgrades/Buy Amount Strategy/Buy X")]
public class BuyXStrategy : BuyAmountStrategy
{
    [SerializeField] private BigDouble _buyAmount = 1;
    public BigDouble BuyAmount { get => _buyAmount; private set => _buyAmount = value; }

    public override BigDouble GetBuyAmount(Upgrade upgrade)
    {
        if (upgrade.Config.hasMaxLevel)
        {
            BigDouble remaining = upgrade.Config.maxLevel - upgrade.CurrentLevel;
            return BigDouble.Min(remaining, BuyAmount);
        }
        return BuyAmount;
    }

    public override BigDouble GetBuyAmount()
    {
        return BuyAmount;
    }

    public override BigDouble GetCost(Upgrade upgrade)
    {
        var upgradeFormula = upgrade.Config.costFormula as ExponentialFormula;
        var exponent = upgradeFormula.Exponent;
        return BigMath.SumGeometricSeries(GetBuyAmount(upgrade), upgrade.Config.baseCost, exponent, upgrade.CurrentLevel);
    }
}
