using System;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class Counter : UpgradeReceiver
{
    [SerializeField] private UpgradeConfig holeUpgrade;
    [SerializeField] private FlyweightSettings floatingTextSettings;
    private BigDouble currentValue;

    protected override void Start()
    {
        base.Start();
    }

    public override BigDouble GetCurrentValue()
    {
        return currentValue;
    }

    protected override void HandlePowerChanged()
    {
        currentValue = upgrade.CurrentPower;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<Ball>(out var ball))
        {
            var finalValue = ball.GetCurrentValue() * GetCurrentValue();

            DataController.Instance.AddPoints(finalValue);
            ShowFloatingText(collider, finalValue);
        }
        if (collider.gameObject.TryGetComponent<Flyweight>(out var flyweight))
        {
            flyweight.Despawn();
        }
    }

    private void ShowFloatingText(Collider collider, BigDouble value)
    {
        //var floatingText = Instantiate(floatingTextPrefab, collider.transform.position, floatingTextPrefab.transform.rotation);
        var floatingText = FlyweightFactory.Spawn(floatingTextSettings);
        //floatingText.transform.SetParent(transform);
        floatingText.transform.SetPositionAndRotation(collider.transform.position, floatingText.transform.rotation);
        floatingText.GetComponent<TextMesh>().text = $"+{value.Notate()}";
    }
}
