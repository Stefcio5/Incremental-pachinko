using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private GameObject floatingTextPrefab;
    [SerializeField]
    private DataScriptableObject data;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;
    [SerializeField]
    private UpgradeScriptableObject boxUpgradeScriptableObject;

    private void OnTriggerEnter(Collider other)
    {
        data.AddPoints(GetAddedPoints());
        ShowFloatingText(other);
        Destroy(other.gameObject, 2f);
    }

    private double GetAddedPoints()
    {
        return upgradeScriptableObject.UpgradePower * boxUpgradeScriptableObject.UpgradePower;
    }

    private void ShowFloatingText(Collider other)
    {
        var floatingText = Instantiate(floatingTextPrefab, other.transform.position, floatingTextPrefab.transform.rotation);
        floatingText.GetComponent<TextMesh>().text = $"+{GetAddedPoints().ToString()}";
    }
}
