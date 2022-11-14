using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DataScriptableObject data;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;
    [SerializeField]
    private UpgradeScriptableObject boxUpgradeScriptableObject;

    private void OnTriggerEnter(Collider other)
    {
        data.AddPoints((upgradeScriptableObject.upgradePower) * boxUpgradeScriptableObject.upgradePower);
        Destroy(other.gameObject, 2f);
    }
}
