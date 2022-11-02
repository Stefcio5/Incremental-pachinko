using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "ScriptableObjects/Data")]
public class DataScriptableObject : ScriptableObject
{
    public double points;

    [System.NonSerialized]
    public UnityEvent<double> pointsChangeEvent;
    private void OnEnable()
    {
        if (pointsChangeEvent == null)
        {
            pointsChangeEvent = new UnityEvent<double>();
        }
    }

    public void AddPoints(double amount)
    {
        points += amount;
        pointsChangeEvent.Invoke(points);
    }
}
