using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    public string upgradeName;
    public double upgradeLevel;
    [SerializeField]
    private double baseUpgradeCost;
    [SerializeField]
    private double upgradeCostMultiplier;
    [SerializeField]
    private double baseUpgradePower;
    [SerializeField]
    private double upgradePowerMultiplier;

    [SerializeField]
    private bool hasMaxLevel;
    [SerializeField]
    private double maxLevel;

    [System.NonSerialized]
    public UnityEvent buyUpgradeEvent;
    [SerializeField]
    private DataScriptableObject playerData;

    [Header("ReadOnly Values")]
    [SerializeField]
    public double upgradeCost;
    [SerializeField]
    private double upgradePower;

    public double MaxLevel { get => maxLevel; private set => maxLevel = value; }
    public double UpgradePower { get => upgradePower; private set => upgradePower = value; }

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

    public double CalculateUpgradeCost(double level)
    {
        upgradeCost = baseUpgradeCost * (1 + level * level) * upgradeCostMultiplier;
        return upgradeCost;
    }

    public double CalculateUpgradePower(double level)
    {
        upgradePower = baseUpgradePower + (level * upgradePowerMultiplier);
        return upgradePower;
    }

}
