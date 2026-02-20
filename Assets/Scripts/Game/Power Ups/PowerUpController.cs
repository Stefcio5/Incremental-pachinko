using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUpController : PersistentSingleton<PowerUpController>
{
    [SerializeField] private List<PowerUpConfig> _powerUps = new();
    [SerializeField] private PowerUpPickup _powerUpPrefab;
    [SerializeField] private float _spawnChance = 0.1f;
    [SerializeField] private PowerUpSpawnPositionFinder _spawnPositionFinder;

    /// <summary>Active powerup modifiers keyed by config. Each config can have multiple effects.</summary>
    private readonly Dictionary<PowerUpConfig, List<(BigDoubleSO target, StatModifier modifier)>> _activePowerUps = new();
    private readonly Dictionary<PowerUpConfig, CancellationTokenSource> _durationTokens = new();

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

    /// <summary>
    /// Try to spawn a random PowerUp pickup in the world.
    /// Returns true when a pickup was successfully instantiated.
    /// </summary>
    public bool TrySpawnPowerUpPrefab()
    {
        if (UnityEngine.Random.Range(0f, 1f) > _spawnChance) return false;
        if (_powerUps.Count == 0 || _spawnPositionFinder is null || _powerUpPrefab is null) return false;

        int index = UnityEngine.Random.Range(0, _powerUps.Count);
        PowerUpConfig selectedPowerUp = _powerUps[index];
        Vector3 spawnPosition = _spawnPositionFinder.GetPowerUpSpawnPosition();
        Instantiate(_powerUpPrefab, spawnPosition, Quaternion.Euler(0, 0, 90))
            .Init(selectedPowerUp);

        return true;
    }

    /// <summary>
    /// Activate a PowerUp, registering all its stat modifiers.
    /// If already active, the duration is refreshed.
    /// </summary>
    public void ActivatePowerUp(PowerUpConfig config)
    {
        if (config is null) return;

        if (_activePowerUps.ContainsKey(config))
        {
            RefreshDuration(config);
            OnPowerUpActivated?.Invoke(config);
            return;
        }

        var registered = new List<(BigDoubleSO target, StatModifier modifier)>();

        foreach (var effect in config.Effects)
        {
            if (effect.Target is null) continue;

            var modifier = new StatModifier(effect.ModifierType, ModifierSource.PowerUp, config.Name, effect.Value);
            effect.Target.AddModifier(modifier);
            registered.Add((effect.Target, modifier));
        }

        _activePowerUps[config] = registered;
        Debug.Log($"[PowerUpController] Activated: {config.Name}");
        OnPowerUpActivated?.Invoke(config);
        StartDurationTask(config);
    }

    private async UniTask PowerUpDurationTask(PowerUpConfig config, CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(config.Duration, cancellationToken: cancellationToken);
        RemovePowerUp(config);
    }

    /// <summary>Deactivate a PowerUp and unregister all its stat modifiers.</summary>
    private void RemovePowerUp(PowerUpConfig config)
    {
        if (config is null || !_activePowerUps.TryGetValue(config, out var registered)) return;

        StopDurationTask(config);

        foreach (var (target, modifier) in registered)
        {
            target.RemoveModifier(modifier);
        }

        _activePowerUps.Remove(config);
        Debug.Log($"[PowerUpController] Deactivated: {config.Name}");
        OnPowerUpDeactivated?.Invoke(config);
    }

    private void StartDurationTask(PowerUpConfig config)
    {
        StopDurationTask(config);
        var cts = new CancellationTokenSource();
        _ = PowerUpDurationTask(config, cts.Token);
        _durationTokens[config] = cts;
    }

    private void StopDurationTask(PowerUpConfig config)
    {
        if (config is null) return;

        if (_durationTokens.TryGetValue(config, out var cts))
        {
            cts?.Cancel();
            cts?.Dispose();
            _durationTokens.Remove(config);
        }
    }

    private void RefreshDuration(PowerUpConfig config)
    {
        StartDurationTask(config);
    }

    private void HandleGameReset()
    {
        ClearActivePowerUps();
        ClearAllPickups();
    }

    private void ClearActivePowerUps()
    {
        var activeConfigs = new List<PowerUpConfig>(_activePowerUps.Keys);
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

        foreach (var cts in _durationTokens.Values)
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        _durationTokens.Clear();
        _activePowerUps.Clear();
    }
}
