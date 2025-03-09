using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] private UpgradeConfig upgradeConfig;
    protected Upgrade upgrade;

    protected virtual void Start()
    {
        upgrade = UpgradeManager.Instance.GetUpgrade(upgradeConfig.upgradeName);
        if (upgrade != null)
        {
            upgrade.OnLevelChanged += (u) => HandlePowerChanged();
            HandlePowerChanged();
        }
    }

    protected abstract void HandlePowerChanged();
    public abstract BigDouble GetCurrentValue();

    protected virtual void OnDestroy()
    {
        upgrade.OnLevelChanged -= (u) => HandlePowerChanged();
    }
}
