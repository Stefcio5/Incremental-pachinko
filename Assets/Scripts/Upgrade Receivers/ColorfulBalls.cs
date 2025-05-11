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

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        ApplyUpgrade();
        upgradePower.onValueChanged += ApplyUpgrade;
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
        // Sort the list by ID before displaying it in the tooltip

        var orderedBallFlyweightSettings = ballFlyweightSettings.OrderBy(x => x.ID).ToList();
        string result = "Spawn Chances:\n";
        foreach (var ballFlyweightSetting in orderedBallFlyweightSettings)
        {
            result += $"{ballFlyweightSetting.name} (x{ballFlyweightSetting.multiplier}): {ballFlyweightSetting.spawnChance}% (+{ballFlyweightSetting.spawnChanceincrement}%)\n";
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
    public void CalculateSpawnChance()
    {
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            Debug.Log($"Ball: {ballFlyweightSetting.name}, Spawn Chance: {ballFlyweightSetting.spawnChance} (+{ballFlyweightSetting.spawnChanceincrement})");
        }
    }

    private void OnDisable()
    {
        upgradePower.onValueChanged -= ApplyUpgrade;
    }
}
