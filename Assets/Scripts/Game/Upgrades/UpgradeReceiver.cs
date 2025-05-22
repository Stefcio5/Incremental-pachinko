using BreakInfinity;
using UnityEngine;

public abstract class UpgradeReceiver : MonoBehaviour
{
    [SerializeField] protected BigDoubleSO upgradePower;

    protected virtual void Awake()
    {
    }
    protected virtual void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (upgradePower == null)
        {
            Debug.LogError("[UpgradeReceiver] UpgradeConfig is not assigned!");
            return;
        }

        OnUpgradeInitialized();
    }

    protected virtual void OnUpgradeInitialized()
    {
    }


    public virtual BigDouble GetUpgradeValue()
    {
        return upgradePower.FinalValue;
    }
}
