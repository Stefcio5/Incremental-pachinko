using BreakInfinity;
using UnityEngine;

public class Counter : UpgradeReceiver
{
    [SerializeField] private FloatingTextFlyweightSettings _floatingTextSettings;
    [SerializeField] private BallSpawnCounterSO _spawnCounter;

    private void OnTriggerEnter(Collider collider)
    {
        // Handle regular balls
        if (collider.gameObject.TryGetComponent<Ball>(out var ball))
        {
            BigDouble finalValue = GetFinalValue(ball);
            DataController.Instance.AddPoints(finalValue);
            ShowFloatingText(collider, ball.BallColor, finalValue, ball.BallID);
        }

        // Handle pooled flyweight objects
        if (collider.gameObject.TryGetComponent<Flyweight>(out var flyweight))
        {
            flyweight.Despawn();
            _spawnCounter.Remove(1);
            PowerUpController.Instance.TrySpawnPowerUpPrefab();
        }
    }

    private BigDouble GetFinalValue(Ball ball)
    {
        return ball.GetUpgradeValue() * upgradePower.FinalValue;
    }

    private void ShowFloatingText(Collider collider, Color color, BigDouble value, int size)
    {
        var floatingText = (FloatingTextFlyweight)FlyweightFactory.Spawn(_floatingTextSettings);
        var spawnLocation = collider.transform.position;
        floatingText.SetText(value.Notate(), color);
        floatingText.AnimateFloatingText(spawnLocation, size);
    }
}
