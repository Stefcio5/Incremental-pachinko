using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class ColorfulBalls : UpgradeReceiver
{
    [SerializeField] private List<BallFlyweightSettings> _ballFlyweightSettings;
    [SerializeField] private BallFlyweightSettings _defaultBallFlyweightSettings;
    [SerializeField] private TooltipText _tooltipText;

    private int _buyAmount = 1;

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        ApplyUpgrade();
        upgradePower.onValueChanged += ApplyUpgrade;
        BuyAmountController.OnBuyAmountStrategyChanged += HandleBuyAmountChanged;
    }

    protected virtual void OnDestroy()
    {
        upgradePower.onValueChanged -= ApplyUpgrade;
        BuyAmountController.OnBuyAmountStrategyChanged -= HandleBuyAmountChanged;
    }

    private void ApplyUpgrade()
    {
        foreach (var ballSetting in _ballFlyweightSettings)
        {
            ballSetting.spawnChance = ballSetting.spawnChanceincrement * (float)upgradePower.DisplayValue;
        }
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        var orderedSettings = _ballFlyweightSettings.OrderBy(x => x.ID).ToList();
        var tooltipBuilder = new System.Text.StringBuilder("Spawn Chances:\n");

        foreach (var ballSetting in orderedSettings)
        {
            if (ballSetting.spawnChance > 100)
            {
                continue;
            }

            float spawnChance = Mathf.Round(ballSetting.spawnChance * 1000f) / 1000f;
            float increment = ballSetting.spawnChanceincrement * _buyAmount;
            string colorHex = ColorUtility.ToHtmlStringRGB(ballSetting.color);

            tooltipBuilder.AppendLine($"<color=#{colorHex}>{ballSetting.name} (x{ballSetting.multiplier}): {spawnChance:G}% (+{increment:G}%)</color>");
        }

        _tooltipText.SetTooltipText(tooltipBuilder.ToString().TrimEnd());
    }

    public BallFlyweightSettings GetRandomBallFlyweightSettings()
    {
        float randomValue = Random.Range(0f, 100f);

        foreach (var ballSetting in _ballFlyweightSettings)
        {
            if (randomValue < ballSetting.spawnChance)
            {
                return ballSetting;
            }
        }

        return _defaultBallFlyweightSettings;
    }

    private void HandleBuyAmountChanged(BuyAmountStrategy buyAmountStrategy)
    {
        _buyAmount = (int)buyAmountStrategy.GetBuyAmount();
        UpdateTooltip();
    }
}
