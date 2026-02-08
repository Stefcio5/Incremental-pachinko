using System;
using BreakInfinity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeUI : MonoBehaviour
{
    // --- UI REFERENCES ---
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI _upgradeLevelText;
    [SerializeField] private Image _upgradeLevelImage;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _buyButtonText;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _unavailableColor;

    // --- INTERNAL REFERENCES ---
    private Upgrade _upgrade;
    private TooltipTrigger _tooltipTrigger;
    private Sequence _animationSequence;

    // --- UNITY EVENTS ---
    private void OnEnable()
    {
        _buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnDisable()
    {
        _buyButton.onClick.RemoveListener(OnBuyClicked);
    }

    private void OnDestroy()
    {
        if (_upgrade?.CurrentPower != null)
            _upgrade.CurrentPower.onValueChanged -= UpdateVisuals;

        DataController.Instance.OnDataChanged -= UpdateVisuals;
        BuyAmountController.OnBuyAmountStrategyChanged -= OnBuyAmountStrategyChanged;
    }

    // --- PUBLIC API ---

    public void SetUpgrade(Upgrade upgrade)
    {
        _upgrade = upgrade;

        if (_upgrade.Config.hasTooltip && _tooltipTrigger == null)
        {
            _tooltipTrigger = gameObject.AddComponent<TooltipTrigger>();
            _tooltipTrigger.followMouse = false;
            _tooltipTrigger.targetOverride = GetComponent<RectTransform>();
        }

        SubscribeToEvents();
        UpdateVisuals();
    }

    public void AnimateUI()
    {
        if (_animationSequence != null && _animationSequence.IsActive())
        {
            _animationSequence.Kill(true);
        }
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0f).SetEase(Ease.OutBack));
        _animationSequence.Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack));
        _animationSequence.OnComplete(() => _animationSequence.Kill(true));
    }

    // --- PRIVATE METHODS ---

    private void SubscribeToEvents()
    {
        if (_upgrade?.CurrentPower != null)
            _upgrade.CurrentPower.onValueChanged += UpdateVisuals;

        DataController.Instance.OnDataChanged += UpdateVisuals;
        BuyAmountController.OnBuyAmountStrategyChanged += OnBuyAmountStrategyChanged;
    }

    private void UpdateVisuals()
    {
        if (_upgrade == null) return;

        _upgradeNameText.text = _upgrade.Config.upgradeName;

        _upgradeDescriptionText.text =
            $"{_upgrade.Config.descriptionPrefix}{_upgrade.CurrentPower.DisplayValue.Notate(_upgrade.Config.notationPrecision)}" +
            $"{_upgrade.Config.descriptionSuffix} <voffset=0.2em>→</voffset> " +
            $"{_upgrade.Config.descriptionPrefix}{_upgrade.GetNextPower().Notate(_upgrade.Config.notationPrecision)}" +
            $"{_upgrade.Config.descriptionSuffix}";

        _upgradeLevelText.text = _upgrade.Config.hasMaxLevel
            ? $"{_upgrade.CurrentLevel}/{_upgrade.Config.maxLevel}"
            : _upgrade.CurrentLevel.Notate();


        _upgradeLevelImage.fillAmount = _upgrade.Config.useStepMultiplier ? _upgrade.GetCurrentStepValue() : 0f;

        var buyAmount = _upgrade.BuyAmountStrategy.GetBuyAmount(_upgrade);
        _upgradeCostText.text = $"Cost: {_upgrade.CurrentCost.Notate()}";
        _buyButtonText.text = $"Buy {buyAmount}";

        _buyButton.interactable = _upgrade.CanPurchase();
        _buyButtonImage.color = _buyButton.interactable ? _defaultColor : _unavailableColor;

        if (_tooltipTrigger != null)
        {
            _tooltipTrigger.content = _upgrade.Config.tooltipText.tooltipText;
            _tooltipTrigger.UpdateText();
        }
    }

    private void OnBuyClicked()
    {
        _upgrade?.Purchase();
        UpdateVisuals();
        AnimateUI();
    }

    private void OnBuyAmountStrategyChanged(BuyAmountStrategy strategy)
    {
        UpdateVisuals();
    }
}
