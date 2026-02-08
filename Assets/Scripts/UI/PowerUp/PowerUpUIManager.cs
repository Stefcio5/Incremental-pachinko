using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIManager : MonoBehaviour
{
    [SerializeField] private Transform _uiHolder;
    [SerializeField] private PowerUpUIElement _uiElementPrefab;

    private readonly Dictionary<PowerUpConfig, PowerUpUIElement> _activeElements = new();

    private void OnEnable()
    {
        if (PowerUpController.HasInstance)
        {
            PowerUpController.Instance.OnPowerUpActivated += HandleActivated;
            PowerUpController.Instance.OnPowerUpDeactivated += HandleDeactivated;
        }
    }

    private void OnDisable()
    {
        if (PowerUpController.HasInstance)
        {
            PowerUpController.Instance.OnPowerUpActivated -= HandleActivated;
            PowerUpController.Instance.OnPowerUpDeactivated -= HandleDeactivated;
        }
    }

    private void HandleActivated(PowerUpConfig config)
    {
        // if (_activeElements.ContainsKey(config))
        // {
        //     Destroy(_activeElements[config].gameObject);
        //     _activeElements.Remove(config);
        // }
        if (!_activeElements.ContainsKey(config))
        {
            var element = Instantiate(_uiElementPrefab, _uiHolder);
            element.Initialize(config, config.Duration);
            _activeElements.Add(config, element);
        }
        else
        {
            _activeElements[config].Initialize(config, config.Duration);
        }

    }

    private void HandleDeactivated(PowerUpConfig config)
    {
        if (_activeElements.TryGetValue(config, out var element))
        {
            Destroy(element.gameObject);
            _activeElements.Remove(config);
        }
    }

    private void OnDestroy()
    {
        foreach (var kvp in _activeElements)
        {
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);
        }
        _activeElements.Clear();
    }
}