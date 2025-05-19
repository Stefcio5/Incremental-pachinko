using System.Runtime.InteropServices.WindowsRuntime;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyXStrategy", menuName = "Upgrades/Buy Amount Strategy/Buy X")]
public class BuyXStrategy : BuyAmountStrategy
{
    public BigDouble buyAmount = 1;
    public override BigDouble GetBuyAmount(Upgrade upgrade)
    {
        if (upgrade.config.hasMaxLevel)
        {
            BigDouble remaining = upgrade.config.maxLevel - upgrade.CurrentLevel;
            return BigDouble.Min(remaining, buyAmount);
        }
        return buyAmount;
    }

    public override BigDouble GetCost(Upgrade upgrade)
    {
        var upgradeFormula = upgrade.config.costFormula as ExponentialFormula;
        var exponent = upgradeFormula.Exponent;
        return BigMath.SumGeometricSeries(GetBuyAmount(upgrade), upgrade.config.baseCost, exponent, upgrade.CurrentLevel);
    }
}
