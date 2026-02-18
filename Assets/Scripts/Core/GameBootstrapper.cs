using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private bool _showDebugLogs = true;
    [SerializeField] private string _mainSceneName = "Main Scene";
    [SerializeField] private List<MonoBehaviour> _gameSystems = new();

    private CancellationTokenSource _initializationCts;
    private bool _isInitialized;

    public static GameBootstrapper Instance { get; private set; }

    public float InitializationProgress { get; private set; }

    public event Action OnInitializationComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        _initializationCts = new CancellationTokenSource();

        try
        {
            await InitializeGameAsync(_initializationCts.Token);
        }
        catch (OperationCanceledException)
        {
            LogDebug("Game initialization was cancelled");
        }
        catch (Exception e)
        {
            Debug.LogError($"Fatal error during game initialization: {e}");
            // Could show error UI here
        }
    }

    private async UniTask InitializeGameAsync(CancellationToken cancellationToken)
    {
        LogDebug("=== Game Initialization Started ===");

        var totalSteps = _gameSystems.Count + 1; // +1 for scene load

        for (int i = 0; i < _gameSystems.Count; i++)
        {
            var systemComponent = _gameSystems[i];

            if (systemComponent == null)
            {
                Debug.LogWarning($"[GameBootstrapper] Game system at index {i} is null, skipping");
                continue;
            }

            if (systemComponent is not IGameSystem system)
            {
                Debug.LogWarning($"[GameBootstrapper] {systemComponent.name} does not implement IGameSystem, skipping");
                continue;
            }

            LogDebug($"Step {i + 1}/{totalSteps}: Initializing {system.SystemName}...");

            await system.InitializeAsync(
                progress: new Progress<float>(p => UpdateProgress(i, totalSteps, p)),
                cancellationToken: cancellationToken
            );

            LogDebug($"{system.SystemName} initialized");
        }

        int sceneStep = _gameSystems.Count;
        LogDebug($"Step {sceneStep + 1}/{totalSteps}: Loading main scene...");
        await LoadMainSceneAsync(
            progress: new Progress<float>(p => UpdateProgress(sceneStep, totalSteps, p)),
            cancellationToken: cancellationToken
        );

        LogDebug("Main scene loaded");

        // Initialization complete
        InitializationProgress = 1f;
        _isInitialized = true;
        LogDebug("=== Game Initialization Complete ===");

        OnInitializationComplete?.Invoke();
    }

    private async UniTask LoadMainSceneAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
    {
        if (SceneManager.GetSceneByName(_mainSceneName).isLoaded)
        {
            LogDebug($"Scene '{_mainSceneName}' already loaded");
            progress?.Report(1f);
            return;
        }

        var loadOperation = SceneManager.LoadSceneAsync(_mainSceneName, LoadSceneMode.Additive);

        if (loadOperation == null)
        {
            Debug.LogError($"Failed to start loading scene '{_mainSceneName}'");
            return;
        }

        while (!loadOperation.isDone)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(loadOperation.progress);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        progress?.Report(1f);
        LogDebug($"Scene '{_mainSceneName}' loaded successfully");
    }

    private void UpdateProgress(int currentStep, int totalSteps, float stepProgress)
    {
        InitializationProgress = (currentStep + stepProgress) / totalSteps;
    }

    private void LogDebug(string message)
    {
        if (_showDebugLogs)
        {
            Debug.Log($"[GameBootstrapper] {message}");
        }
    }

    private void OnDestroy()
    {
        _initializationCts?.Cancel();
        _initializationCts?.Dispose();

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
