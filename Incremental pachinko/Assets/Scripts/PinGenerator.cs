using System.Collections.Generic;
using UnityEngine;

public class PinGenerator : MonoBehaviour
{
    public GameObject pinPrefab;
    public Transform pinParent;

    [Header("Settings")]
    [Header("Pozycja w osi X")]
    public float pinX = 0.0f;
    public float minZ = -20f;
    public float maxZ = 20f;
    public float minY = 7.0f;
    public float maxY = 25.0f;

    [Header("Spacing")]
    public float spacingY = 2.0f;
    public float spacingZ = 2.85f;

    private List<GameObject> spawnedPins = new List<GameObject>();

    void Start()
    {
        GeneratePins();
    }

    [ContextMenu("Generate Pins")]
    public void GeneratePins()
    {
        ClearPins();

        for (float y = minY; y <= maxY; y += spacingY)
        {
            float zOffset = ((int)((y - minY) / spacingY) % 2 == 0) ? 0f : spacingZ * 0.5f;

            for (float z = minZ + spacingZ * 0.5f + zOffset; z <= maxZ - spacingZ * 0.5f; z += spacingZ)
            {
                SpawnPin(new Vector3(pinX, y, z));
            }
        }
    }

    private void SpawnPin(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        GameObject pin = Instantiate(pinPrefab, position, rotation, pinParent);
        spawnedPins.Add(pin);
    }

    [ContextMenu("Clear Pins")]
    private void ClearPins()
    {
        foreach (var p in spawnedPins)
        {
            if (p != null)
                DestroyImmediate(p);
        }
        spawnedPins.Clear();
    }
}
