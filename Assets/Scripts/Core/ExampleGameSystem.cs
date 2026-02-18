using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ExampleGameSystem : PersistentSingleton<ExampleGameSystem>, IGameSystem
{
    [SerializeField] private float _initializationDelay = 0.5f;
    [SerializeField] private bool _showLogs = true;

    private bool _isInitialized;

    public string SystemName => "ExampleGameSystem";
    public bool IsInitialized => _isInitialized;
    public event Action OnSystemInitialized;

    protected override void Awake()
    {
        base.Awake();
        LogDebug("Awake called");
    }

    public async UniTask InitializeAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            LogDebug("Already initialized, skipping");
            return;
        }

        LogDebug("Starting initialization...");
        progress?.Report(0f);

        try
        {
            // Phase 1: Load resources
            await LoadResourcesAsync(cancellationToken);
            progress?.Report(0.33f);

            // Phase 2: Setup dependencies
            await SetupDependenciesAsync(cancellationToken);
            progress?.Report(0.66f);

            // Phase 3: Validate
            ValidateInitialization();
            progress?.Report(1f);

            _isInitialized = true;
            LogDebug("Initialization complete");

            OnSystemInitialized?.Invoke();
        }
        catch (OperationCanceledException)
        {
            LogDebug("Initialization cancelled");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[{SystemName}] Initialization failed: {e.Message}");
            throw;
        }
    }

    private async UniTask LoadResourcesAsync(CancellationToken cancellationToken)
    {
        LogDebug("Loading resources...");

        // Simulate resource loading with cancellation support
        await UniTask.Delay(TimeSpan.FromSeconds(_initializationDelay), cancellationToken: cancellationToken);

        LogDebug("Resources loaded");
    }

    private async UniTask SetupDependenciesAsync(CancellationToken cancellationToken)
    {
        LogDebug("Setting up dependencies...");

        // Check if required systems are initialized
        if (!DataController.Instance.IsInitialized)
        {
            throw new InvalidOperationException($"{SystemName} requires DataController to be initialized first");
        }

        // Simulate dependency setup
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

        LogDebug("Dependencies set up");
    }

    private void ValidateInitialization()
    {
        LogDebug("Validating initialization...");

        // Add your validation logic here
        // Throw exceptions if validation fails

        LogDebug("Validation passed");
    }

    public void DoSomething()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"[{SystemName}] Attempted to use before initialization");
            return;
        }

        LogDebug("Doing something...");
        // Your logic here
    }

    public async UniTask DoSomethingAsync(CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"[{SystemName}] Attempted to use before initialization");
            return;
        }

        LogDebug("Doing something async...");

        await UniTask.Delay(100, cancellationToken: cancellationToken);

        LogDebug("Async operation complete");
    }

    private void LogDebug(string message)
    {
        if (_showLogs)
        {
            Debug.Log($"[{SystemName}] {message}");
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LogDebug("OnDestroy called");
    }
}
