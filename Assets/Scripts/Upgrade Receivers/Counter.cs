using System;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class Counter : UpgradeReceiver
{
    [SerializeField] private FlyweightSettings floatingTextSettings;

    protected override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<Ball>(out var ball))
        {
            BigDouble finalValue = GetFinalValue(ball);

            DataController.Instance.AddPoints(finalValue);
            ShowFloatingText(collider, finalValue);
        }
        if (collider.gameObject.TryGetComponent<Flyweight>(out var flyweight))
        {
            flyweight.Despawn();
        }
    }

    private BigDouble GetFinalValue(Ball ball)
    {
        return ball.GetUpgradeValue() * upgradePower.FinalValue;
    }

    private void ShowFloatingText(Collider collider, BigDouble value)
    {
        var floatingText = FlyweightFactory.Spawn(floatingTextSettings);
        var spawnLocation = collider.transform.position + Vector3.right * 3f;
        floatingText.transform.SetPositionAndRotation(spawnLocation, floatingText.transform.rotation);

        if (floatingText.TryGetComponent<TextMesh>(out var textMesh))
        {
            textMesh.text = "+" + value.Notate();
        }
    }
}
