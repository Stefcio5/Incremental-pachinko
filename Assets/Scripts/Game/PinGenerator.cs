using System.Collections.Generic;
using UnityEngine;

public class PinGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _pinPrefab;
    [SerializeField] private Transform _pinParent;

    [Header("Settings")]
    [SerializeField] private float _pinX = 0.0f;
    [SerializeField] private float _minZ = -20f;
    [SerializeField] private float _maxZ = 20f;
    [SerializeField] private float _minY = 7.0f;
    [SerializeField] private float _maxY = 25.0f;

    [Header("Spacing")]
    [SerializeField] private float _spacingY = 2.0f;
    [SerializeField] private float _spacingZ = 2.85711f;

    private List<GameObject> _spawnedPins = new List<GameObject>();

    void Start()
    {
        GeneratePins();
    }

    [ContextMenu("Generate Pins")]
    public void GeneratePins()
    {
        ClearPins();

        for (float y = _minY; y <= _maxY; y += _spacingY)
        {
            float zOffset = ((int)((y - _minY) / _spacingY) % 2 == 0) ? 0f : _spacingZ * 0.5f;

            for (float z = _minZ + _spacingZ * 0.5f + zOffset; z <= _maxZ - _spacingZ * 0.5f; z += _spacingZ)
            {
                SpawnPin(new Vector3(_pinX, y, z));
            }
        }
    }

    private void SpawnPin(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        GameObject pin = Instantiate(_pinPrefab, position, rotation, _pinParent);
        _spawnedPins.Add(pin);
    }

    [ContextMenu("Clear Pins")]
    private void ClearPins()
    {
        foreach (var p in _spawnedPins)
        {
            if (p != null)
                DestroyImmediate(p);
        }
        _spawnedPins.Clear();
    }
}
