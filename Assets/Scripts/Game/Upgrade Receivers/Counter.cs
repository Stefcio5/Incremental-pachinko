using BreakInfinity;
using UnityEngine;

public class Counter : UpgradeReceiver
{
    [SerializeField] private FloatingTextFlyweightSettings _floatingTextSettings;

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
            ShowFloatingText(collider, ball.BallColor, finalValue, ball.BallID);
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

    private void ShowFloatingText(Collider collider, Color color, BigDouble value, int size)
    {
        FloatingTextFlyweight floatingText = (FloatingTextFlyweight)FlyweightFactory.Spawn(_floatingTextSettings);
        var spawnLocation = collider.transform.position;
        floatingText.SetText(value.Notate(), color);
        floatingText.AnimateFloatingText(spawnLocation, size);
    }
}
