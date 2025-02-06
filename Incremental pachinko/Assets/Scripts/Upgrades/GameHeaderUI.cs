using UnityEngine;
using UnityEngine.UI;

public class GameHeaderUI : MonoBehaviour
{
    [SerializeField] private Text _pointsText;
    [SerializeField] private Text _prestigePointsText;

    private void Start()
    {
        UpdateUI();
        DataController.Instance.OnDataChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        _pointsText.text = $"Points: {DataController.Instance.CurrentGameData.points.Notate()}";
        _prestigePointsText.text = $"Prestige Points: {DataController.Instance.CurrentGameData.prestigePoints.Notate()}";
    }

}
