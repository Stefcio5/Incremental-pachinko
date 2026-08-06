using UnityEngine;

/// <summary>
/// Entry point for game initialization.
/// Instantiates the Systems prefab which contains GameBootstrapper and core systems.
/// </summary>
public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var systemsPrefab = Resources.Load("Systems");

        if (systemsPrefab == null)
        {
            Debug.LogError("[Bootstrapper] Failed to load 'Systems' prefab from Resources. Make sure it exists.");
            return;
        }

        Object.Instantiate(systemsPrefab);
        Debug.Log("[Bootstrapper] Systems instantiated successfully");
    }
}
