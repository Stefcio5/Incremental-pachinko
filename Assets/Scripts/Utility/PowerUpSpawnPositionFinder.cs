using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PowerUpSpawnPositionFinder : MonoBehaviour
{
    private List<Transform> _spawnPoints = new();

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _spawnPoints.Clear();
        _spawnPoints = new List<Transform>(GetComponentsInChildren<Transform>());

        _spawnPoints.Remove(transform);
    }

    [ContextMenu("Get PowerUp Spawn Transform")]
    public Vector3 GetPowerUpSpawnPosition()
    {
        if (_spawnPoints.Count < 2)
        {
            Debug.LogWarning("Not enough spawn points.");
            return Vector3.zero;
        }
        var selectedPinTransform = GetRandomPinPosition();

        Vector3 nearestPin = FindNearestPinPosition(selectedPinTransform);

        if (nearestPin == Vector3.zero)
        {
            return selectedPinTransform;
        }

        Vector3 midpoint = (selectedPinTransform + nearestPin) / 2f;

        return midpoint;
    }

    private Vector3 GetRandomPinPosition()
    {
        if (_spawnPoints.Count == 0) return Vector3.zero;
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index].position;
    }

    private Vector3 FindNearestPinPosition(Vector3 reference)
    {
        Vector3 nearest = Vector3.zero;
        float minDistance = float.MaxValue;

        foreach (var pin in _spawnPoints)
        {
            // Pomiń samego siebie
            if (pin.position == reference) continue;

            float distance = Vector3.Distance(reference, pin.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = pin.position;
            }
        }

        return nearest;
    }
}
