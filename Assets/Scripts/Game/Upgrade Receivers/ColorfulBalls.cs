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
        BuyAmountController.OnBuyAmountStrategyChanged += GetBuyAmount;
    }

    private void ApplyUpgrade()
    {
        foreach (var ballFlyweightSetting in _ballFlyweightSettings)
        {
            ballFlyweightSetting.spawnChance = ballFlyweightSetting.spawnChanceincrement * (float)upgradePower.FinalValue;
        }
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        var orderedBallFlyweightSettings = _ballFlyweightSettings.OrderBy(x => x.ID).ToList();
        string result = "Spawn Chances:\n";
        foreach (var ballFlyweightSetting in orderedBallFlyweightSettings)
        {
            if (ballFlyweightSetting.spawnChance > 100) continue;
            float spawnChance = Mathf.Round(ballFlyweightSetting.spawnChance * 1000f) / 1000f;
            float increment = ballFlyweightSetting.spawnChanceincrement * _buyAmount;
            result += $"<color=#{ColorUtility.ToHtmlStringRGB(ballFlyweightSetting.color)}>{ballFlyweightSetting.name} (x{ballFlyweightSetting.multiplier}): {spawnChance:G}% (+{increment:G}%)</color>\n";
        }
        _tooltipText.SetTooltipText(result);
    }


    public BallFlyweightSettings GetRandomBallFlyweightSettings()
    {
        float randomValue = Random.Range(0f, 100f);

        foreach (var ballFlyweightSetting in _ballFlyweightSettings)
        {
            if (randomValue < ballFlyweightSetting.spawnChance)
            {
                return ballFlyweightSetting;
            }
        }

        return _defaultBallFlyweightSettings;
    }
    private void GetBuyAmount(BuyAmountStrategy buyAmountStrategy)
    {
        if (buyAmountStrategy == null)
        {
            _buyAmount = 1;
        }
        else
        {
            _buyAmount = (int)buyAmountStrategy.GetBuyAmount();
        }
        UpdateTooltip();
    }

    private void OnDisable()
    {
        upgradePower.onValueChanged -= ApplyUpgrade;
    }

    private void OnDestroy()
    {
        BuyAmountController.OnBuyAmountStrategyChanged -= GetBuyAmount;
    }
}
