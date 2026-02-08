using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PowerUpController : PersistentSingleton<PowerUpController>
{
    [SerializeField] private List<PowerUpConfig> _powerUps = new List<PowerUpConfig>();
    [SerializeField] private PowerUpPickup _powerUpPrefab;
    [SerializeField] private float _spawnChance = 0.1f;
    [SerializeField] private PowerUpSpawnPositionFinder _spawnPositionFinder;

    private readonly Dictionary<PowerUpConfig, Coroutine> _durationCoroutines = new();

    public event Action<PowerUpConfig> OnPowerUpActivated;
    public event Action<PowerUpConfig> OnPowerUpDeactivated;

    private void OnEnable()
    {
        var dataController = DataController.TryGetInstance();
        if (dataController != null)
        {
            dataController.OnGameReset += HandleGameReset;
        }
    }

    private void OnDisable()
    {
        var dataController = DataController.TryGetInstance();
        if (dataController != null)
        {
            dataController.OnGameReset -= HandleGameReset;
        }
    }

    public bool TrySpawnPowerUpPrefab()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > _spawnChance) return false;

        if (_powerUps.Count == 0) return false;
        if (_spawnPositionFinder is null) return false;
        if (_powerUpPrefab is null) return false;

        int index = UnityEngine.Random.Range(0, _powerUps.Count);
        PowerUpConfig selectedPowerUp = _powerUps[index];
        Vector3 spawnPosition = _spawnPositionFinder.GetPowerUpSpawnPosition();
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        Instantiate(_powerUpPrefab, spawnPosition, rotation)
            .Init(selectedPowerUp);
        return true;
    }


    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         if (_powerUps.Count > 0)
    //         {
    //             int index = UnityEngine.Random.Range(0, _powerUps.Count);
    //             ActivatePowerUp(_powerUps[index]);
    //         }
    //     }
    // }

    public void ActivatePowerUp(PowerUpConfig config)
    {
        if (config is null) return;
        if (config.Target is null) return;
        // Only one instance of each power-up type can be active at a time
        //if (config.Target.HasPowerUp(config)) return;

        if (config.Target.HasPowerUp(config))
        {
            RefreshDuration(config);
            OnPowerUpActivated?.Invoke(config);
            return;
        }

        config.Target.AddPowerUp(config);
        Debug.Log($"Activated PowerUp: {config.Name}");
        Debug.Log($"New Target Value: {config.Target.FinalValue}");

        OnPowerUpActivated?.Invoke(config);
        StartDurationCoroutine(config);
    }

    private IEnumerator PowerUpDurationCoroutine(PowerUpConfig config)
    {
        yield return new WaitForSeconds(config.Duration);
        RemovePowerUp(config);
    }

    private void RemovePowerUp(PowerUpConfig config)
    {
        if (config is null || config.Target is null) return;
        StopDurationCoroutine(config);
        config.Target.RemovePowerUp(config);
        Debug.Log($"Deactivated PowerUp: {config.Name}");
        Debug.Log($"New Target Value: {config.Target.FinalValue}");

        OnPowerUpDeactivated?.Invoke(config);
    }

    private void StartDurationCoroutine(PowerUpConfig config)
    {
        StopDurationCoroutine(config);
        Coroutine coroutine = StartCoroutine(PowerUpDurationCoroutine(config));
        _durationCoroutines[config] = coroutine;
    }

    private void StopDurationCoroutine(PowerUpConfig config)
    {
        if (config is null) return;

        if (_durationCoroutines.TryGetValue(config, out Coroutine coroutine))
        {
            if (coroutine is not null)
            {
                StopCoroutine(coroutine);
            }
            _durationCoroutines.Remove(config);
        }
    }

    private void RefreshDuration(PowerUpConfig config)
    {
        StartDurationCoroutine(config);
    }

    private void HandleGameReset()
    {
        ClearActivePowerUps();
        ClearAllPickups();
    }

    private void ClearActivePowerUps()
    {
        var activeConfigs = new List<PowerUpConfig>(_durationCoroutines.Keys);
        foreach (var config in activeConfigs)
        {
            RemovePowerUp(config);
        }
    }

    private void ClearAllPickups()
    {
        var pickups = FindObjectsByType<PowerUpPickup>(FindObjectsSortMode.None);
        foreach (var pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
        _durationCoroutines.Clear();
    }
}
