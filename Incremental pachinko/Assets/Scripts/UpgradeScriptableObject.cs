using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    //public Image upgradeButton;
    //public TMP_Text levelText;
    //public TMP_Text nameText;
    //public TMP_Text costText;

    public string upgradeName;
    public double level;
    public double upgradeCost;
    [SerializeField]
    private double baseUpgradePower;
    public double upgradePower;

    [System.NonSerialized]
    public UnityEvent<double> buyUpgradeEvent;

    private void OnEnable()
    {
        if (buyUpgradeEvent == null)
        {
            buyUpgradeEvent = new UnityEvent<double>();
        }
        CalculateUpgradeCost(level);
        CalculateUpgradePower(level);
    }

    public void BuyUpgrade()
    {
        level++;
        CalculateUpgradeCost(level);
        CalculateUpgradePower(level);
        buyUpgradeEvent.Invoke(level);
    }

    public double CalculateUpgradeCost(double level)
    {
        upgradeCost = 10 * (1 + level);
        return upgradeCost;
    }

    public double CalculateUpgradePower(double level)
    {
        if (level.Equals(0))
        {
            upgradePower = 1;
            return upgradePower;
        }
        else
        {
            upgradePower = (level * baseUpgradePower);
            return upgradePower;
        }
    }

}
