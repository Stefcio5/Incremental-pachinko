using BreakInfinity;

public class PrestigePointsUpgradeStrategy : IUpgradePurchaseStrategy
{
    public bool CanPurchase(BigDouble cost) => DataController.Instance.CurrentGameData.prestigePoints >= cost;


    public void PurchaseUpgrade(BigDouble cost) => DataController.Instance.SpendPrestigePoints(cost);

}