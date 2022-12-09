using BreakInfinity;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private DataScriptableObject dataScriptableObject;

    // Start is called before the first frame update
    void Start()
    {
        ChangePointsText();
    }
    private void OnEnable()
    {
        dataScriptableObject.pointsChangeEvent.AddListener(ChangePointsText);
    }
    private void OnDisable()
    {
        dataScriptableObject.pointsChangeEvent.RemoveListener(ChangePointsText);
    }

    private void ChangePointsText()
    {
        pointsText.text = $"Points: {dataScriptableObject.points.Notate()}";
    }
}
