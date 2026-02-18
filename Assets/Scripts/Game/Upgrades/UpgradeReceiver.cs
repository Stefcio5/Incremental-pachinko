using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] protected BigDoubleSO upgradePower;

    private bool _isInitialized;

    protected virtual void Awake()
    {
    }

    protected virtual void OnEnable()
    {
        SubscribeToUpgradeManager();
        TryInitialize();
    }

    protected virtual void Start()
    {
        TryInitialize();
    }

    protected virtual void OnDisable()
    {
        UnsubscribeFromUpgradeManager();
    }

    private void TryInitialize()
    {
        if (_isInitialized)
        {
            return;
        }

        if (upgradePower is null)
        {
            Debug.LogError($"[{nameof(UpgradeReceiver)}] {nameof(upgradePower)} is not assigned!");
            return;
        }

        var manager = UpgradeManager.Instance;
        if (manager is null || !manager.IsInitialized)
        {
            return;
        }

        _isInitialized = true;

        OnUpgradeInitialized();
    }

    protected virtual void OnUpgradeInitialized()
    {
    }

    public virtual BigDouble GetUpgradeValue()
    {
        return upgradePower.DisplayValue;
    }

    private void SubscribeToUpgradeManager()
    {
        var manager = UpgradeManager.Instance;
        if (manager is null)
        {
            Debug.LogWarning($"[{nameof(UpgradeReceiver)}] {nameof(UpgradeManager)} instance not available yet.");
            return;
        }

        manager.OnSystemInitialized -= HandleUpgradeManagerInitialized;
        manager.OnSystemInitialized += HandleUpgradeManagerInitialized;
    }

    private void UnsubscribeFromUpgradeManager()
    {
        var manager = UpgradeManager.Instance;
        if (manager is null)
        {
            return;
        }

        manager.OnSystemInitialized -= HandleUpgradeManagerInitialized;
    }

    private void HandleUpgradeManagerInitialized()
    {
        TryInitialize();
    }
}
