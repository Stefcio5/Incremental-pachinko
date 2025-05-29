using DG.Tweening;
using UnityEngine;

public class SettingsButtonUI : MonoBehaviour
{
    [SerializeField] private float _scaleOnHover = .6f;
    [SerializeField] private float _rotationDuration = 10f;

    private Vector3 _originalScale;
    private Tween _scaleTween;
    private Tween _rotateTween;
    void Start()
    {
        _originalScale = transform.localScale;
    }

    public void OnMouseEnter()
    {
        _scaleTween?.Kill();
        _rotateTween?.Kill();

        _scaleTween = transform.DOScale(_scaleOnHover, 0.2f).SetEase(Ease.OutQuad);

        _rotateTween = transform.DOLocalRotate(new Vector3(0, 0, -360), _rotationDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    public void OnMouseExit()
    {
        _scaleTween?.Kill();
        _rotateTween?.Kill();

        _scaleTween = transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad);
    }

}
