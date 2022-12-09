using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField]
    private Image upgradeButton;
    [SerializeField]
    private Color DefaultColor;
    [SerializeField]
    private Color UnavailableColor;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text costText;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;
    [SerializeField]
    private DataScriptableObject playerData;

    void Start()
    {
        ChangeLevelText();
        ChangeCostText();
        ChangeUpgradeNameText();
        UpdateButtonColor();
    }
    private void Update()
    {
        UpdateButtonColor();
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

    
    private void ChangeLevelText()
    {
        if (upgradeScriptableObject.HasMaxLevel())
        {
            levelText.text = $"{upgradeScriptableObject.upgradeLevel.ToString()}/{upgradeScriptableObject.MaxLevel}";
        }
        else
        {
            levelText.text = upgradeScriptableObject.upgradeLevel.ToString();
        }
    }

    private void ChangeCostText() => costText.text = $"Cost: {upgradeScriptableObject.upgradeCost.Notate()}";

    private void ChangeUpgradeNameText() => nameText.text = $"{upgradeScriptableObject.upgradeName}{upgradeScriptableObject.UpgradePower}";

    public void UpdateButtonColor()
    {
        ChangeButtonColor(upgradeButton, upgradeScriptableObject.CanBuyUpgrade() ? DefaultColor : UnavailableColor);
    }
    private void ChangeButtonColor(Image button, Color color)
    {
        button.color = color;
    }
}
