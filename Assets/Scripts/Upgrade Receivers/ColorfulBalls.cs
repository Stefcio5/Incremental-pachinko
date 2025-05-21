using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

public class ColorfulBalls : UpgradeReceiver
{
    [SerializeField] private List<BallFlyweightSettings> ballFlyweightSettings;
    [SerializeField] private BallFlyweightSettings defaultBallFlyweightSettings;
    public TooltipText tooltipText;
    private int buyAmount = 1;

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        ApplyUpgrade();
        upgradePower.onValueChanged += ApplyUpgrade;
        BuyAmountController.OnBuyAmountStrategyChanged += GetBuyAmount;
    }

    private void ApplyUpgrade()
    {
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            ballFlyweightSetting.spawnChance = ballFlyweightSetting.spawnChanceincrement * (float)upgradePower.FinalValue;
        }
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        var orderedBallFlyweightSettings = ballFlyweightSettings.OrderBy(x => x.ID).ToList();
        string result = "Spawn Chances:\n";
        foreach (var ballFlyweightSetting in orderedBallFlyweightSettings)
        {
            if (ballFlyweightSetting.spawnChance > 100) continue;
            result += $"{ballFlyweightSetting.name} (x{ballFlyweightSetting.multiplier}): {ballFlyweightSetting.spawnChance}% (+{ballFlyweightSetting.spawnChanceincrement * buyAmount}%)\n";
        }
        tooltipText.SetTooltipText(result);
    }


    public BallFlyweightSettings GetRandomBallFlyweightSettings()
    {
        float randomValue = Random.Range(0f, 100f);

        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            if (randomValue < ballFlyweightSetting.spawnChance)
            {
                return ballFlyweightSetting;
            }
        }

        return defaultBallFlyweightSettings;
    }
    private void GetBuyAmount(BuyAmountStrategy buyAmountStrategy)
    {
        if (buyAmountStrategy == null)
        {
            buyAmount = 1;
        }
        else
        {
            buyAmount = (int)buyAmountStrategy.GetBuyAmount();
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
