using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Image upgradeButton;
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text costText;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;

    [SerializeField]
    private Text ballPowerText;
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private DataScriptableObject dataScriptableObject;

    void Start()
    {
        ChangeLevelText(upgradeScriptableObject.level);
        ChangeCostText(upgradeScriptableObject.upgradeCost);
        ChangeBallPowerText(upgradeScriptableObject.upgradePower);
        ChangePointsText(dataScriptableObject.points);
    }

    //TODO: Change buyUpgradeEvent to pointsChangeEvent
    private void OnEnable()
    {
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeLevelText);
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeCostText);
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeBallPowerText);

        dataScriptableObject.pointsChangeEvent.AddListener(ChangePointsText);
    }
    private void OnDisable()
    {
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeLevelText);
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeCostText);
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeBallPowerText);

        dataScriptableObject.pointsChangeEvent.RemoveListener(ChangePointsText);
    }

    private void ChangeLevelText(double level)
    {
        levelText.text = level.ToString();
    }

    private void ChangeCostText(double level)
    {
        costText.text = $"Cost: {upgradeScriptableObject.upgradeCost}";
    }
    
    private void ChangeBallPowerText(double level)
    {
        ballPowerText.text = $"Ball multi: x{upgradeScriptableObject.upgradePower}";
    }

    private void ChangePointsText(double points)
    {
        pointsText.text = $"Points: {points}";
    }
}
