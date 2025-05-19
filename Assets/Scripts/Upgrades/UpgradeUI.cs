using System;
using BreakInfinity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI _upgradeLevelText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _unavailableColor;

    private Upgrade _upgrade;
    private TooltipTrigger _tooltipTrigger;
    private void OnEnable()
    {
        _buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnDisable()
    {
        _buyButton.onClick.RemoveListener(OnBuyClicked);
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        _upgrade = upgrade;
        if (_upgrade.config.hasTooltip)
        {
            _tooltipTrigger = gameObject.AddComponent<TooltipTrigger>();
        }
        UpdateVisuals();
        _upgrade.CurrentPower.onValueChanged += UpdateVisuals;
        DataController.Instance.OnDataChanged += UpdateVisuals;
    }

    private void OnDestroy()
    {
        _buyButton.onClick.RemoveListener(OnBuyClicked);
        _upgrade.CurrentPower.onValueChanged -= UpdateVisuals;
        DataController.Instance.OnDataChanged -= UpdateVisuals;
    }

    private void UpdateVisuals()
    {
        _upgradeNameText.text = $"{_upgrade.config.upgradeName}";
        _upgradeDescriptionText.text = $"{_upgrade.config.upgradeDescription}{_upgrade.CurrentPower.FinalValue.Notate(_upgrade.config.notationPrecision)}{_upgrade.config.descriptionSuffix}";
        _upgradeLevelText.text = _upgrade.config.hasMaxLevel
            ? $"{_upgrade.CurrentLevel}/{_upgrade.config.maxLevel}"
            : $"{_upgrade.CurrentLevel.Notate()}";
        _upgradeCostText.text = $"Cost: {_upgrade.CurrentCost.Notate()}";

        _buyButton.interactable = _upgrade.CanPurchase();
        _buyButtonImage.color = _buyButton.interactable ? _defaultColor : _unavailableColor;

        if (_tooltipTrigger != null)
        {
            _tooltipTrigger.content = _upgrade.config.tooltipText.tooltipText;
        }
    }

    private void OnBuyClicked()
    {
        _upgrade.Purchase();
        UpdateVisuals();
    }
    // private string Notate(BigDouble value)
    // {
    //     return value.Notate(_upgrade.config.notationPrecision);
    // }
}
