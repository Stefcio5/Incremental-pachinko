using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Upgrades/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    [Header("Base Settings")]
    public string upgradeName;
    public string upgradeDescription;
    [Tooltip("Optional text to add before upgradeDescription")]
    public string descriptionPrefix;
    [Tooltip("Optional text to add after upgradeDescription")]
    public string descriptionSuffix;
    [Header("Level Configuration")]
    [SerializeField] private BigDouble _startingLevel = 0;
    [Tooltip("Starting level for this upgrade. Useful when you want level 0 to have a base value.")]
    public BigDouble StartingLevel => _startingLevel;
    [Header("Upgrade Type")]
    public UpgradeType upgradeType;

    [Header("Upgrade Settings")]
    public BigDouble baseCost;
    public UpgradeFormula costFormula;
    public BuyAmountStrategy buyAmountStrategy;

    [Header("Upgrade Effects")]
    public BigDouble basePower;
    public BigDoubleSO upgradePower;
    public UpgradeFormula powerFormula;

    [Header("Optional Power Multiplier")]
    public bool useStepMultiplier = false;
    public BigDouble multiplierInterval = 50;
    public BigDouble multiplierBase = 2;

    [Header("Upgrade Limitations")]
    public bool hasMaxLevel;
    public BigDouble maxLevel;

    [Tooltip("Number of decimals to show for values less than 1")]
    public int notationPrecision = 0;

    public bool hasTooltip;
    public TooltipText tooltipText;
}
