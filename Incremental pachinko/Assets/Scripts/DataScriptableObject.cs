using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "ScriptableObjects/Data")]
public class DataScriptableObject : ScriptableObject
{
    public BigDouble points;
    [System.NonSerialized]
    public UnityEvent pointsChangeEvent;
    private void OnEnable()
    {
        if (pointsChangeEvent == null)
        {
            pointsChangeEvent = new UnityEvent();
        }
    }

    public void AddPoints(BigDouble amount)
    {
        points += amount;
        pointsChangeEvent.Invoke();
    }
    public void ResetPoints()
    {
        points = 0;
        pointsChangeEvent.Invoke();
    }
}
