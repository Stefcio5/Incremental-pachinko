using System;
using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] private UpgradeConfig upgradeConfig;
    protected Upgrade upgrade;
    protected BigDouble Value => upgrade != null ? upgrade.CurrentPower : 0;

    protected virtual void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        upgrade = UpgradeManager.Instance.GetUpgrade(upgradeConfig.upgradeName);
        Debug.Log($"UpgradeReceiver initialized with upgrade: {upgradeConfig.upgradeName}");
    }


    public virtual BigDouble GetValue()
    {
        return Value;
    }

    protected virtual void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnInitialized -= Initialize;
        }
    }
}
