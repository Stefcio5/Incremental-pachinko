using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyAmountController : MonoBehaviour
{
    [SerializeField] private BuyAmountStrategy _currentBuyAmountStrategy;
    [SerializeField] private BuyAmountStrategy[] _buyAmountStrategies;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;
    private int _currentBuyAmountIndex = 0;

    public static event Action<BuyAmountStrategy> OnBuyAmountStrategyChanged;

    private void Start()
    {
        if (_currentBuyAmountStrategy == null)
        {
            _currentBuyAmountStrategy = _buyAmountStrategies[0];
        }

        UpdateButtonText();
        _button.onClick.AddListener(SwitchStrategy);
    }

    private void SwitchStrategy()
    {
        _currentBuyAmountIndex++;
        if (_currentBuyAmountIndex >= _buyAmountStrategies.Length)
        {
            _currentBuyAmountIndex = 0;
        }

        _currentBuyAmountStrategy = _buyAmountStrategies[_currentBuyAmountIndex];
        OnBuyAmountStrategyChanged?.Invoke(_currentBuyAmountStrategy);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        _buttonText.text = $"{_currentBuyAmountStrategy.name}";
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(SwitchStrategy);
    }
}
