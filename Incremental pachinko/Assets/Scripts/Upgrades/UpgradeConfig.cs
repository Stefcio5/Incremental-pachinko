using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Upgrades/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    [Header("Base Settings")]
    public string upgradeName;
    public string upgradeDescription;
    public UpgradeType upgradeType;

    [Header("Upgrade Settings")]
    public BigDouble baseCost;
    public ExponentialFormula costFormula;

    [Header("Upgrade Effects")]
    public BigDouble basePower;
    public LinearFormula powerFormula;

    [Header("Upgrade Limitations")]
    public bool hasMaxLevel;
    public BigDouble maxLevel;
}
