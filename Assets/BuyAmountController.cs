using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyAmountController : MonoBehaviour
{
    [SerializeField] private BuyAmountStrategy currentBuyAmountStrategy;
    [SerializeField] private BuyAmountStrategy[] buyAmountStrategies;
    private int currentBuyAmountIndex = 0;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    public static event Action<BuyAmountStrategy> OnBuyAmountStrategyChanged;

    private void Start()
    {
        if (currentBuyAmountStrategy == null)
        {
            currentBuyAmountStrategy = buyAmountStrategies[0];
        }

        UpdateButtonText();
        button.onClick.AddListener(SwitchStrategy);
    }

    private void SwitchStrategy()
    {
        currentBuyAmountIndex++;
        if (currentBuyAmountIndex >= buyAmountStrategies.Length)
        {
            currentBuyAmountIndex = 0;
        }

        currentBuyAmountStrategy = buyAmountStrategies[currentBuyAmountIndex];
        OnBuyAmountStrategyChanged?.Invoke(currentBuyAmountStrategy);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        buttonText.text = $"{currentBuyAmountStrategy.name}";
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(SwitchStrategy);
    }
}
