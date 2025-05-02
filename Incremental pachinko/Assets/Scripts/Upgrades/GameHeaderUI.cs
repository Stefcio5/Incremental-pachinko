using System;
using UnityEngine;
using UnityEngine.UI;

public class GameHeaderUI : MonoBehaviour
{
    [SerializeField] private Text _pointsText;
    [SerializeField] private Text _prestigePointsText;
    [SerializeField] private Button _prestigeButton;

    private void Start()
    {
        UpdateUI();
        DataController.Instance.OnDataChanged += UpdateUI;
        _prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
    }

    private void OnPrestigeButtonClicked()
    {
        DataController.Instance.PrestigeGame();
    }

    private void UpdateUI()
    {
        _pointsText.text = $"Points: {DataController.Instance.CurrentGameData.points.Notate()}";
        _prestigePointsText.text = $"Prestige Points: {DataController.Instance.CurrentGameData.prestigePoints.Notate()} (+{DataController.Instance.CalculatePrestige().Notate()})";
        _prestigeButton.interactable = DataController.Instance.CalculatePrestige() > 0;
    }
    private void OnDestroy()
    {
        DataController.Instance.OnDataChanged -= UpdateUI;
        _prestigeButton.onClick.RemoveListener(OnPrestigeButtonClicked);
    }

}
