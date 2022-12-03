using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    public string upgradeName;
    public BigDouble upgradeLevel;
    [SerializeField]
    private BigDouble baseUpgradeCost;
    [SerializeField]
    private BigDouble upgradeCostMultiplier;
    [SerializeField]
    private BigDouble baseUpgradePower;
    [SerializeField]
    private BigDouble upgradePowerMultiplier;

    [SerializeField]
    private bool hasMaxLevel;
    [SerializeField]
    private BigDouble maxLevel;

    [System.NonSerialized]
    public UnityEvent buyUpgradeEvent;
    [SerializeField]
    private DataScriptableObject playerData;

    [Header("ReadOnly Values")]
    [SerializeField]
    public BigDouble upgradeCost;
    [SerializeField]
    private BigDouble upgradePower;

    public BigDouble MaxLevel { get => maxLevel; private set => maxLevel = value; }
    public BigDouble UpgradePower { get => upgradePower; private set => upgradePower = value; }

    private void OnEnable()
    {
        if (buyUpgradeEvent == null)
        {
            buyUpgradeEvent = new UnityEvent();
        }
        CalculateUpgradeCost(upgradeLevel);
        CalculateUpgradePower(upgradeLevel);
    }

    public void BuyUpgrade()
    {
        if (HasMaxLevel() && IsUpgradeLevelLowerThanMax() && CanBuyUpgrade() || !hasMaxLevel && CanBuyUpgrade())
        {
            playerData.AddPoints(-upgradeCost);
            upgradeLevel++;
            CalculateUpgradeCost(upgradeLevel);
            CalculateUpgradePower(upgradeLevel);
            buyUpgradeEvent.Invoke();
        }
    }
    public void ResetUpgrade()
    {
        upgradeLevel = 0;
        CalculateUpgradeCost(upgradeLevel);
        CalculateUpgradePower(upgradeLevel);
        buyUpgradeEvent.Invoke();
    }

    private bool CanBuyUpgrade()
    {
        return playerData.points >= upgradeCost;
    }

    private bool IsUpgradeLevelLowerThanMax()
    {
        return upgradeLevel < maxLevel;
    }

    public bool HasMaxLevel()
    {
        return hasMaxLevel;
    }

    public BigDouble CalculateUpgradeCost(BigDouble level)
    {
        upgradeCost = baseUpgradeCost * BigDouble.Pow(upgradeCostMultiplier, level);
        return upgradeCost;
    }

    public BigDouble CalculateUpgradePower(BigDouble level)
    {
        
        upgradePower = baseUpgradePower + (upgradePowerMultiplier * level);
        return upgradePower;

    }

}
