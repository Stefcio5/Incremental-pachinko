using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// Interface for game systems that require initialization.
/// Provides async initialization with progress reporting and cancellation support.
/// </summary>
public interface IGameSystem
{
    /// <summary>
    /// Name of the system for logging and debugging.
    /// </summary>
    string SystemName { get; }

    /// <summary>
    /// Indicates whether the system has completed initialization.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initialize the system asynchronously.
    /// </summary>
    /// <param name="progress">Optional progress reporter for initialization status (0.0 to 1.0).</param>
    /// <param name="cancellationToken">Token to cancel initialization.</param>
    /// <returns>UniTask that completes when initialization is finished.</returns>
    UniTask InitializeAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event fired when system initialization completes.
    /// </summary>
    event Action OnSystemInitialized;
}
