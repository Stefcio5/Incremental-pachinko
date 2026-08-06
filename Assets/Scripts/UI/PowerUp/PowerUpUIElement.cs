using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _multiplierText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _fillImage;

    private PowerUpConfig _config;
    private float _endTime;
    private Coroutine _updateRoutine;

    public void Initialize(PowerUpConfig config, float duration)
    {
        _config = config;
        _endTime = Time.time + duration;

        _nameText.text = config.Name;
        _multiplierText.text = BuildEffectSummary(config);

        if (_updateRoutine != null) StopCoroutine(_updateRoutine);
        _updateRoutine = StartCoroutine(UpdateTimerRoutine());
    }

    private IEnumerator UpdateTimerRoutine()
    {
        while (Time.time < _endTime)
        {
            float remaining = _endTime - Time.time;
            _timerText.text = remaining.ToString("F1") + "s";

            if (_fillImage != null)
            {
                float t = remaining / _config.Duration;
                _fillImage.fillAmount = Mathf.Clamp01(t);
            }

            yield return null;
        }

        _timerText.text = "0.0s";
        if (_fillImage != null) _fillImage.fillAmount = 0f;
    }

    private static string BuildEffectSummary(PowerUpConfig config)
    {
        if (config.Effects == null || config.Effects.Count == 0)
            return string.Empty;

        if (config.Effects.Count == 1)
        {
            var e = config.Effects[0];
            return e.ModifierType == ModifierType.Multiplicative
                ? $"x{e.Value}"
                : $"+{e.Value}";
        }

        var parts = new System.Text.StringBuilder();
        foreach (var e in config.Effects)
        {
            if (parts.Length > 0) parts.Append(", ");
            parts.Append(e.ModifierType == ModifierType.Multiplicative ? $"x{e.Value}" : $"+{e.Value}");
        }
        return parts.ToString();
    }

    private void OnDestroy()
    {
        if (_updateRoutine != null) StopCoroutine(_updateRoutine);
    }
}