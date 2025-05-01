using BreakInfinity;
using UnityEngine;

public interface IUpgradePurchaseStrategy
{
    bool CanPurchase(BigDouble cost);
    void PurchaseUpgrade(BigDouble cost);
}
