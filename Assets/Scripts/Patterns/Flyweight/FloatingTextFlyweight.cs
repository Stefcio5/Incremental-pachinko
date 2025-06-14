using DG.Tweening;
using UnityEngine;

public class FloatingTextFlyweight : Flyweight
{
    new FloatingTextFlyweightSettings settings => (FloatingTextFlyweightSettings)base.settings;
    [SerializeField] Vector3 _spawnOffset = new Vector3(3f, 1f, 0);
    [SerializeField] private Vector3 _randomTweenOffset;
    private TextMesh _textMesh;
    private Sequence _sequence;
    private Vector3 _initialPosition;
    private Vector3 _initialScale;

    private void Awake()
    {
        if (_textMesh == null)
        {
            _textMesh = GetComponent<TextMesh>();
        }
        _initialPosition = transform.position;
        _initialScale = transform.localScale;
    }

    public void SetText(string text, Color color)
    {
        if (_textMesh != null)
        {
            _textMesh.color = color;
            _textMesh.text = "+" + text;
        }
        else
        {
            Debug.LogWarning("TextMesh component not found on FloatingText.");
        }
    }

    public void AnimateFloatingText(Vector3 position, int strength)
    {
        transform.position = position + _spawnOffset;
        transform.localScale = Vector3.one * 0.1f;

        var baseScale = strength + 1f;
        var scale = Mathf.Pow(baseScale, 0.7f) * 0.15f;

        var offset = GetRandomOffset();

        if (_sequence != null)
        {
            _sequence.Kill(true);
            _sequence = null;
        }

        _sequence = DOTween.Sequence()
            .Append(transform.DOScale(scale, 0.5f).SetEase(Ease.InQuint))
            .Join(transform.DOMoveY(transform.position.y + offset.y, 0.3f).SetEase(Ease.OutQuint))
            .Join(transform.DOMoveZ(transform.position.z + offset.z, 0.5f).SetEase(Ease.OutQuint))
            .Join(transform.DOMoveX(transform.position.x + 1f, 0.5f).SetEase(Ease.Linear))
            .Append(transform.DOScale(0f, 0.3f).SetEase(Ease.InQuad))
            .OnComplete(Despawn);

        _sequence.Play();
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-_randomTweenOffset.x, _randomTweenOffset.x),
                           Random.Range(1f, _randomTweenOffset.y),
                           Random.Range(-_randomTweenOffset.z, _randomTweenOffset.z));

    }

    public override void Despawn()
    {
        if (!gameObject.activeSelf) return;
        FlyweightFactory.ReturnToPool(this);
    }

    private void OnDisable()
    {
        transform.position = _initialPosition;
        transform.localScale = _initialScale;
        _sequence?.Kill(true);
        _sequence = null;
    }
}