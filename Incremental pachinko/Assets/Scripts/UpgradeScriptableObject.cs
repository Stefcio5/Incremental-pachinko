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
    private double upgradeMultiplier;
    [SerializeField]
    public double upgradeCost;
    [SerializeField]
    private double baseUpgradePower;
    public double upgradePower;

    [System.NonSerialized]
    public UnityEvent buyUpgradeEvent;
    [SerializeField]
    private DataScriptableObject playerData;

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
        if (playerData.points >= upgradeCost)
        {
            playerData.AddPoints(-upgradeCost);
            upgradeLevel++;
            CalculateUpgradeCost(upgradeLevel);
            CalculateUpgradePower(upgradeLevel);
            buyUpgradeEvent.Invoke();
        }
    }

    public double CalculateUpgradeCost(double level)
    {
        upgradeCost = baseUpgradeCost * (1 + level * level) * upgradeMultiplier;
        return upgradeCost;
    }

    public double CalculateUpgradePower(double level)
    {
        upgradePower = baseUpgradePower + (level * baseUpgradePower);
        return upgradePower;
    }

}
