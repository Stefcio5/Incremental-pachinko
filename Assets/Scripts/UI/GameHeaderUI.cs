using BreakInfinity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameHeaderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private TextMeshProUGUI _prestigePointsText;
    [SerializeField] private TextMeshProUGUI _ballCountText;
    [SerializeField] private TextMeshProUGUI _pointsToPrestigeText;
    [SerializeField] private Button _prestigeButton;
    [SerializeField] private UpgradeConfig _prestigeUpgradeConfig;
    [SerializeField] private BallSpawnCounterSO _ballSpawnCounter;

    private void Start()
    {
        UpdateUI();
        DataController.Instance.OnDataChanged += UpdateUI;
        _prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
        _prestigeUpgradeConfig.upgradePower.onValueChanged += () => UpdateUI();
        _ballSpawnCounter.OnCountChanged.AddListener(UpdateBallCountUI);
        UpdateBallCountUI(_ballSpawnCounter.CurrentCount);
    }

    private void OnPrestigeButtonClicked()
    {
        DataController.Instance.PrestigeGame();
    }

    private void UpdateUI()
    {
        _pointsText.text = $"Points: {DataController.Instance.CurrentGameData.points.Notate()}";
        _prestigePointsText.text = $"Prestige Points: {DataController.Instance.CurrentGameData.prestigePoints.Notate()} (+{DataController.Instance.CalculatePrestige().Notate()})";
        _pointsToPrestigeText.text = $"Next prestige point: {DataController.Instance.PointsToNextPrestige().Notate()}";
        _prestigeButton.interactable = DataController.Instance.CalculatePrestige() >= 1;
    }

    private void UpdateBallCountUI(BigDouble currentCount)
    {
        _ballCountText.text = $"Balls: {currentCount.Notate()} / {_ballSpawnCounter.MaxCount.DisplayValue.Notate()}";
    }
    private void OnDestroy()
    {
        DataController.Instance.OnDataChanged -= UpdateUI;
        _prestigeButton.onClick.RemoveListener(OnPrestigeButtonClicked);
        _prestigeUpgradeConfig.upgradePower.onValueChanged -= () => UpdateUI();
        _ballSpawnCounter.OnCountChanged.RemoveListener(UpdateBallCountUI);
    }

    public void OnPrestigeButtonPointerEnter()
    {
        if (!_prestigeButton.IsInteractable())
        {
            _prestigeButton.transform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
            return;
        }
        _prestigeButton.transform.DOScale(1.1f, 0.25f).SetEase(Ease.OutQuad);
    }

    public void OnPrestigeButtonPointerExit()
    {
        if (!_prestigeButton.IsInteractable())
        {
            _prestigeButton.transform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
            return;
        }
        _prestigeButton.transform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
    }
}
