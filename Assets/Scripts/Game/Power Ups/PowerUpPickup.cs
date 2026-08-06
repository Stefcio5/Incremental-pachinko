using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    private const int MillisecondsDelay = 30000;
    [SerializeField] private PowerUpConfig _powerUpConfig;

    public void Init(PowerUpConfig config)
    {
        _powerUpConfig = config;
    }

    void OnEnable()
    {
        _ = SelfDestructTask(this.GetCancellationTokenOnDestroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ball>(out _))
        {
            PowerUpController.Instance.ActivatePowerUp(_powerUpConfig);
            Destroy(gameObject);
        }
    }

    private async UniTask SelfDestructTask(CancellationToken cancellationToken)
    {
        await UniTask.Delay(MillisecondsDelay, cancellationToken: cancellationToken);
        if (this != null) // Still valid?
        {
            Destroy(gameObject);
        }
    }
}
