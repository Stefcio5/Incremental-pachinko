using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optional loading screen UI that displays initialization progress.
/// Attach to a Canvas in the Bootstrap scene to show loading progress to users.
/// </summary>
public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _statusText;

    [Header("Settings")]
    [SerializeField] private bool _hideWhenComplete = true;
    [SerializeField] private float _smoothSpeed = 5f;

    private float _targetProgress;
    private float _currentProgress;

    private void Start()
    {
        if (GameBootstrapper.Instance != null)
        {
            GameBootstrapper.Instance.OnInitializationComplete += OnInitializationComplete;
            UpdateProgressAsync().Forget();
        }
        else
        {
            Debug.LogWarning("[LoadingScreenUI] GameBootstrapper instance not found");
        }
    }

    private async UniTaskVoid UpdateProgressAsync()
    {
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(true);
        }

        while (GameBootstrapper.Instance != null && !GameBootstrapper.Instance.InitializationProgress.Equals(1f))
        {
            _targetProgress = GameBootstrapper.Instance.InitializationProgress;

            // Smooth progress bar animation
            _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * _smoothSpeed);

            UpdateUI(_currentProgress);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // Ensure we show 100% at the end
        UpdateUI(1f);
    }

    private void UpdateUI(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.value = progress;
        }

        if (_progressText != null)
        {
            _progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }

        if (_statusText != null)
        {
            _statusText.text = GetStatusMessage(progress);
        }
    }

    private string GetStatusMessage(float progress)
    {
        if (progress < 0.33f)
            return "Loading game data...";
        else if (progress < 0.66f)
            return "Initializing upgrades...";
        else if (progress < 1f)
            return "Loading scene...";
        else
            return "Complete!";
    }

    private void OnInitializationComplete()
    {
        if (_hideWhenComplete && _loadingPanel != null)
        {
            _loadingPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (GameBootstrapper.Instance != null)
        {
            GameBootstrapper.Instance.OnInitializationComplete -= OnInitializationComplete;
        }
    }
}
