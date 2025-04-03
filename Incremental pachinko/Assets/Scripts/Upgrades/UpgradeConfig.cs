using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Upgrades/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    [Header("Base Settings")]
    public string upgradeName;
    public string upgradeDescription;
    [Tooltip("Optional text to add after upgradeDescription")]
    public string descriptionSuffix;
    public UpgradeType upgradeType;

    [Header("Upgrade Settings")]
    public BigDouble baseCost;
    public UpgradeFormula costFormula;

    [Header("Upgrade Effects")]
    public BigDouble basePower;
    public UpgradeFormula powerFormula;

    [Header("Upgrade Limitations")]
    public bool hasMaxLevel;
    public BigDouble maxLevel;

    [Tooltip("Number of decimals to show for values less than 1")]
    public int notationPrecision = 0;
}
