using DG.Tweening;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void OnEnableAnimation()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnDisableAnimation()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }
}
