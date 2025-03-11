using System;
using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] private UpgradeConfig upgradeConfig;
    protected Upgrade upgrade;

    protected virtual void Start()
    {
        Initialize();
        if (upgrade == null)
        {
            UpgradeManager.Instance.OnInitialized += Initialize;
        }
    }

    protected void Initialize()
    {
        upgrade = UpgradeManager.Instance.GetUpgrade(upgradeConfig.upgradeName);
        if (upgrade != null)
        {
            upgrade.OnLevelChanged += (u) => HandlePowerChanged();
            HandlePowerChanged();
            Debug.Log($"UpgradeReceiver initialized with upgrade: {upgradeConfig.upgradeName}");
        }
    }

    protected abstract void HandlePowerChanged();
    public abstract BigDouble GetCurrentValue();

    protected virtual void OnDestroy()
    {
        upgrade.OnLevelChanged -= (u) => HandlePowerChanged();
        UpgradeManager.Instance.OnInitialized -= Initialize;
    }
}
