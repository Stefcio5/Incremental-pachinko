using DG.Tweening;
using UnityEngine;

public class SpawnRange : UpgradeReceiver
{
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float elasticity = 0.8f;
    [SerializeField] private Ease punchEase = Ease.OutBack;

    private GameObject _spawnRangeObject;
    private Renderer _spawnRangeRenderer;
    private Color _defaultColor;
    private Vector3 _currentScale;
    private Sequence _spawnRangeSequence;

    public Vector3 GetCurrentScale()
    {
        return new Vector3(1f, 1f, (float)upgradePower.FinalValue * 2);
    }

    protected override void Awake()
    {
        base.Awake();
        _spawnRangeRenderer = GetComponent<Renderer>();
        _defaultColor = _spawnRangeRenderer.material.color;
    }
    protected override void Start()
    {
        base.Start();
        _spawnRangeObject = gameObject;
        SetObjectScale();
        OnSpawnRangeUpgradeLevelChanged();
    }

    private void OnSpawnRangeUpgradeLevelChanged()
    {
        upgradePower.onValueChanged += AnimateSpawnRange;
    }
    private void SetObjectScale()
    {
        _spawnRangeObject.transform.localScale = GetCurrentScale();
    }

    public void AnimateSpawnRange()
    {
        _spawnRangeObject.transform.DOScaleZ((float)upgradePower.FinalValue * 2, 0.5f).SetEase(Ease.OutBack);
    }

    public void ChangeSpawnRangeColor(Color color)
    {
        _spawnRangeRenderer.material.DOColor(color, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _spawnRangeRenderer.material.DOColor(_defaultColor, 0.5f).SetEase(Ease.InExpo);
        });
    }

    public void DoPunchScale(int punchStrength)
    {
        _spawnRangeSequence?.Kill();
        _spawnRangeSequence = DOTween.Sequence();
        transform.localScale = GetCurrentScale();

        float scaled = Mathf.Pow(punchStrength, 2f) * 0.08f;
        var punchVector = Vector3.one * scaled;
        _spawnRangeSequence.Append(_spawnRangeObject.transform.DOPunchScale(punchVector, punchDuration, vibrato, elasticity)
                               .SetEase(punchEase));
        //_spawnRangeSequence.Append(_spawnRangeObject.transform.DOScale(GetCurrentScale(), 0.5f).SetEase(Ease.InQuad));


        //_spawnRangeSequence.Append(_spawnRangeObject.transform.DOScale(GetCurrentScale(), 0.2f).SetEase(Ease.OutBack));
    }
}
