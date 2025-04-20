using System;
using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] private UpgradeConfig upgradeConfig;
    protected Upgrade Upgrade { get; private set; }
    protected BigDouble Value => Upgrade != null ? Upgrade.CurrentPower : 0;

    protected virtual void Awake()
    {
        Debug.Log($"UpgradeReceiver awake with upgrade: {upgradeConfig.upgradeName}");
    }
    protected virtual void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (upgradeConfig == null)
        {
            Debug.LogError("[UpgradeReceiver] UpgradeConfig is not assigned!");
            return;
        }
        Upgrade = UpgradeManager.Instance.GetUpgrade(upgradeConfig.upgradeName);
        if (Upgrade == null)
        {
            Debug.LogError($"[UpgradeReceiver] Upgrade not found for name: {upgradeConfig.upgradeName}");
            return;
        }

        Debug.Log($"[UpgradeReceiver] Initialized with upgrade: {upgradeConfig.upgradeName}");
        OnUpgradeInitialized();
    }

    protected virtual void OnUpgradeInitialized()
    {
    }


    public virtual BigDouble GetUpgradeValue()
    {
        return Value;
    }
}
