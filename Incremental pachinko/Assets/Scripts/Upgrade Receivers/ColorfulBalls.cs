using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

public class ColorfulBalls : UpgradeReceiver
{
    [SerializeField] private List<BallFlyweightSettings> ballFlyweightSettings;
    [SerializeField] private BallFlyweightSettings defaultBallFlyweightSettings;
    public TooltipText tooltipText;

    protected override void Start()
    {
        base.Start();
        Init();
        upgrade.OnLevelChanged += (u) => ApplyUpgrade(upgrade.CurrentLevel);
        upgrade.OnLevelChanged += (u) => CheckDebug();
    }

    void OnEnable()
    {
    }

    private void Init()
    {
        ApplyUpgrade(upgrade.CurrentLevel);
    }
    public void CheckDebug()
    {
        Debug.Log("Received upgrade event");
    }

    public void ApplyUpgrade(BigDouble level)
    {
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            ballFlyweightSetting.spawnChance = ballFlyweightSetting.spawnChanceincrement * (float)level;
        }
        CalculateSpawnChance();
        SetSpawnChanceText();
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

    public void SetSpawnChanceText()
    {
        string result = "Spawn Chances:\n";
        foreach (var ballFlyweightSetting in ballFlyweightSettings)
        {
            result += $"{ballFlyweightSetting.name}: {ballFlyweightSetting.spawnChance}% (+{ballFlyweightSetting.spawnChanceincrement})\n";
        }
        tooltipText.SetTooltipText(result);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetSpawnChanceText();
            CalculateSpawnChance();
        }
    }

    private void OnDisable()
    {
        if (upgrade == null) return;
        upgrade.OnLevelChanged -= (u) => ApplyUpgrade(upgrade.CurrentLevel);
    }
}
