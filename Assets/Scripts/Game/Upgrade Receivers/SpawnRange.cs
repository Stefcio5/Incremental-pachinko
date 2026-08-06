using DG.Tweening;
using UnityEngine;

public class SpawnRange : UpgradeReceiver
{
    [SerializeField] private float _punchDuration = 0.3f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _elasticity = 0.8f;
    [SerializeField] private Ease _punchEase = Ease.OutBack;

    private Renderer _spawnRangeRenderer;
    private Color _defaultColor;
    private Sequence _spawnRangeSequence;

    protected override void Awake()
    {
        base.Awake();
        _spawnRangeRenderer = GetComponent<Renderer>();

        if (_spawnRangeRenderer is null)
        {
            Debug.LogError($"[{nameof(SpawnRange)}] Renderer component not found.");
            return;
        }

        _defaultColor = _spawnRangeRenderer.material.color;
    }
    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        SetObjectScale();
        upgradePower.onValueChanged += AnimateSpawnRange;
    }

    protected virtual void OnDestroy()
    {
        _spawnRangeSequence?.Kill();
        upgradePower.onValueChanged -= AnimateSpawnRange;
    }

    public Vector3 GetCurrentScale()
    {
        return new Vector3(1f, 1f, (float)upgradePower.DisplayValue * 2);
    }

    private void SetObjectScale()
    {
        transform.localScale = GetCurrentScale();
    }

    public void AnimateSpawnRange()
    {
        float newScaleZ = (float)upgradePower.DisplayValue * 2;
        transform.DOScaleZ(newScaleZ, 0.5f).SetEase(Ease.OutBack);
    }

    public void ChangeSpawnRangeColor(Color color)
    {
        _spawnRangeRenderer.material.DOColor(color, 0.5f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                _spawnRangeRenderer.material.DOColor(_defaultColor, 0.5f)
                    .SetEase(Ease.InExpo);
            });
    }

    public void DoPunchScale(int punchStrength)
    {
        _spawnRangeSequence?.Kill();

        _spawnRangeSequence = DOTween.Sequence();
        transform.localScale = GetCurrentScale();

        float punch = Mathf.Pow(punchStrength, 2f) * 0.08f;
        var punchVector = Vector3.one * punch;

        _spawnRangeSequence.Append(
            transform.DOPunchScale(punchVector, _punchDuration, _vibrato, _elasticity)
                .SetEase(_punchEase)
        );
    }
}
