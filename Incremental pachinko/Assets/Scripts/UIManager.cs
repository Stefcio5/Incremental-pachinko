using System.Collections;
using System.Collections.Generic;
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
        ChangePointsText(dataScriptableObject.points);
    }
    private void OnEnable()
    {
        dataScriptableObject.pointsChangeEvent.AddListener(ChangePointsText);
    }
    private void OnDisable()
    {
        dataScriptableObject.pointsChangeEvent.RemoveListener(ChangePointsText);
    }

    private void ChangePointsText(double points)
    {
        pointsText.text = $"Points: {points}";
    }
}
