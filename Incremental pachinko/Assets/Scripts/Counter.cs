using System;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private UpgradeConfig holeUpgrade;
    [SerializeField] private GameObject floatingTextPrefab;




    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<Ball>(out var ball))
        {
            var value = ball.GetValue() *
                UpgradeManager.Instance.GetUpgrades(UpgradeType.Basic)
                    .First(u => u.config == holeUpgrade)
                    .CurrentPower;


            DataController.Instance.AddPoints(value);
            ShowFloatingText(collider, value);
        }
        if (collider.gameObject.TryGetComponent<Flyweight>(out var flyweight))
        {
            flyweight.Despawn();
        }
    }

    private void ShowFloatingText(Collider collider, BigDouble value)
    {
        var floatingText = Instantiate(floatingTextPrefab, collider.transform.position, floatingTextPrefab.transform.rotation);
        floatingText.GetComponent<TextMesh>().text = $"+{value.Notate()}";
    }


    // [SerializeField]
    // private GameObject floatingTextPrefab;
    // [SerializeField]
    // private DataScriptableObject data;
    // [SerializeField]
    // private UpgradeScriptableObject upgradeScriptableObject;
    // [SerializeField]
    // private UpgradeScriptableObject boxUpgradeScriptableObject;

    // private void OnTriggerEnter(Collider other)
    // {
    //     data.AddPoints(GetAddedPoints());
    //     ShowFloatingText(other);
    //     Destroy(other.gameObject, 2f);
    // }

    // private BigDouble GetAddedPoints()
    // {
    //     return upgradeScriptableObject.UpgradePower * boxUpgradeScriptableObject.UpgradePower;
    // }

    // private void ShowFloatingText(Collider other)
    // {
    //     var floatingText = Instantiate(floatingTextPrefab, other.transform.position, floatingTextPrefab.transform.rotation);
    //     floatingText.GetComponent<TextMesh>().text = $"+{GetAddedPoints().Notate()}";
    // }
}
