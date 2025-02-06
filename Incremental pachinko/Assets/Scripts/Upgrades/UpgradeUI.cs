using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text _upgradeNameText;
    [SerializeField] private Text _upgradeDescriptionText;
    [SerializeField] private Text _upgradeLevelText;
    [SerializeField] private Text _upgradeCostText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _unavailableColor;

    private Upgrade _upgrade;

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
        UpdateVisuals();
        _upgrade.OnLevelChanged += (u) => UpdateVisuals();
        DataController.Instance.OnDataChanged += UpdateVisuals;
    }

    private void OnDestroy()
    {
        _buyButton.onClick.RemoveListener(OnBuyClicked);
        //_upgrade.OnLevelChanged -= (u) => UpdateVisuals();
        DataController.Instance.OnDataChanged -= UpdateVisuals;
    }

    private void UpdateVisuals()
    {
        _upgradeNameText.text = $"{_upgrade.config.upgradeName} x{_upgrade.CurrentPower}";
        _upgradeDescriptionText.text = _upgrade.config.upgradeDescription;
        _upgradeLevelText.text = $"{_upgrade.CurrentLevel}";
        _upgradeCostText.text = $"Cost: {_upgrade.CurrentCost.Notate()}";

        _buyButton.interactable = DataController.Instance.CurrentGameData.points >= _upgrade.CurrentCost;
        _buyButtonImage.color = _buyButton.interactable ? _defaultColor : _unavailableColor;
    }

    private void OnBuyClicked()
    {
        if (DataController.Instance.SpendPoints(_upgrade.CurrentCost))
        {
            // Buy the upgrade
            _upgrade.LevelUp();
            UpdateVisuals();
        }
    }
}
