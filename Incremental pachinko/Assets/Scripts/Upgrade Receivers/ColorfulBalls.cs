using System.Collections.Generic;
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
        ApplyUpgrade(Upgrade.CurrentLevel);
        Upgrade.OnLevelChanged += (u) => ApplyUpgrade(u.CurrentLevel);
    }

    private void ApplyUpgrade(BigDouble level)
    {
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            ballFlyweightSetting.spawnChance = ballFlyweightSetting.spawnChanceincrement * (float)level;
        }
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        string result = "Spawn Chances:\n";
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            result += $"{ballFlyweightSetting.name}: {ballFlyweightSetting.spawnChance:F2}% (+{ballFlyweightSetting.spawnChanceincrement:F2})\n";
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
        if (Upgrade == null) return;
        Upgrade.OnLevelChanged -= (u) => ApplyUpgrade(Upgrade.CurrentLevel);
    }
}
