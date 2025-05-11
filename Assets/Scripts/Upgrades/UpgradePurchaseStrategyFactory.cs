public static class UpgradePurchaseStrategyFactory
{
    public static IUpgradePurchaseStrategy Create(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.Prestige => new PrestigePointsUpgradeStrategy(),
            _ => new PointsUpgradeStrategy()
        };
    }
}