using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField]
    private Image upgradeButton;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text costText;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;

    void Start()
    {
        ChangeLevelText();
        ChangeCostText();
        ChangeUpgradeNameText();
    }

    //TODO: Change buyUpgradeEvent to pointsChangeEvent
    private void OnEnable()
    {
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeLevelText);
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeCostText);
        upgradeScriptableObject.buyUpgradeEvent.AddListener(ChangeUpgradeNameText);
    }
    private void OnDisable()
    {
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeLevelText);
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeCostText);
        upgradeScriptableObject.buyUpgradeEvent.RemoveListener(ChangeUpgradeNameText);
    }
    private void ChangeLevelText() => levelText.text = upgradeScriptableObject.upgradeLevel.ToString();

    private void ChangeCostText() => costText.text = $"Cost: {upgradeScriptableObject.upgradeCost}";

    private void ChangeUpgradeNameText() => nameText.text = $"{upgradeScriptableObject.upgradeName}{upgradeScriptableObject.upgradePower}";
}
