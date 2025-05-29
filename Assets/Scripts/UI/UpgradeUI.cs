using System;
using BreakInfinity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private TextMeshProUGUI _buyButtonText;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _unavailableColor;

    private Upgrade _upgrade;
    private TooltipTrigger _tooltipTrigger;
    private Sequence _animationSequence;
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
        if (_upgrade.Config.hasTooltip)
        {
            _tooltipTrigger = gameObject.AddComponent<TooltipTrigger>();
        }
        UpdateVisuals();
        _upgrade.CurrentPower.onValueChanged += UpdateVisuals;
        DataController.Instance.OnDataChanged += UpdateVisuals;
        BuyAmountController.OnBuyAmountStrategyChanged += OnBuyAmountStrategyChanged;
    }

    private void OnDestroy()
    {
        _upgrade.CurrentPower.onValueChanged -= UpdateVisuals;
        DataController.Instance.OnDataChanged -= UpdateVisuals;
        BuyAmountController.OnBuyAmountStrategyChanged -= OnBuyAmountStrategyChanged;
    }

    private void UpdateVisuals()
    {
        _upgradeNameText.text = $"{_upgrade.Config.upgradeName}";
        _upgradeDescriptionText.text = $"{_upgrade.Config.upgradeDescription}{_upgrade.CurrentPower.FinalValue.Notate(_upgrade.Config.notationPrecision)}{_upgrade.Config.descriptionSuffix}";
        _upgradeLevelText.text = _upgrade.Config.hasMaxLevel
            ? $"{_upgrade.CurrentLevel}/{_upgrade.Config.maxLevel}"
            : $"{_upgrade.CurrentLevel.Notate()}";

        var buyAmount = _upgrade.BuyAmountStrategy.GetBuyAmount(_upgrade);
        var cost = _upgrade.CurrentCost;

        _upgradeCostText.text = $"Cost: {cost.Notate()}";
        _buyButtonText.text = $"Buy {buyAmount}";
        _buyButton.interactable = _upgrade.CanPurchase();
        _buyButtonImage.color = _buyButton.interactable ? _defaultColor : _unavailableColor;

        if (_tooltipTrigger != null)
        {
            _tooltipTrigger.content = _upgrade.Config.tooltipText.tooltipText;
        }
    }

    private void OnBuyAmountStrategyChanged(BuyAmountStrategy strategy)
    {
        UpdateVisuals();
    }

    private void OnBuyClicked()
    {
        _upgrade.Purchase();
        UpdateVisuals();
        AnimateUI();
    }
    public void AnimateUI()
    {
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0f)).SetEase(Ease.OutBack);
        _animationSequence.Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack)).
        OnComplete(() =>
        {
            _animationSequence.Kill(true);
        });

    }
}
