using System.Collections;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private PowerUpConfig _powerUpConfig;

    public void Init(PowerUpConfig config)
    {
        _powerUpConfig = config;
    }

    void OnEnable()
    {
        StartCoroutine(SelfDestructCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ball>(out _))
        {
            PowerUpController.Instance.ActivatePowerUp(_powerUpConfig);
            Destroy(gameObject);
        }
    }

    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
}
