using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public PlayerData playerData;
    public Upgrades ballUpgrade;

    public string ballUpgradeName;

    public double ballUpgradeBaseCost;
    public double ballUpgradeCostMulti;
    // Start is called before the first frame update
    void Start()
    {
        ballUpgradeName = "Points Per Ball";
        ballUpgradeBaseCost = 10;
        ballUpgradeCostMulti = 1.5;
        playerData = GameObject.Find("Player Data").GetComponent<PlayerData>();
        UpdateUI();
    }

    public double Cost() => (double)(ballUpgradeBaseCost * Mathf.Pow((float)ballUpgradeCostMulti, (float)playerData.ballUpgradeLevel));

    public void BuyUpgrade()
    {
        if (playerData.points >= Cost())    
        {
            playerData.points -= Cost();
            playerData.ballUpgradeLevel += 1;
        }
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        ballUpgrade.levelText.text = playerData.ballUpgradeLevel.ToString();
        ballUpgrade.costText.text = ($"Cost: {Cost().ToString("F0")} points");
        ballUpgrade.nameText.text = ($"+1 {ballUpgradeName}");
    }
}
