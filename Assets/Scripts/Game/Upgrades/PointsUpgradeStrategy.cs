using BreakInfinity;

public class PointsUpgradeStrategy : IUpgradePurchaseStrategy
{
    public bool CanPurchase(BigDouble cost) => DataController.Instance.CurrentGameData.points >= cost;

    public void PurchaseUpgrade(BigDouble cost) => DataController.Instance.SpendPoints(cost);

}