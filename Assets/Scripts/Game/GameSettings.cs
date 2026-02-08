using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private BallSpawnCounterSO _ballSpawnCounter;
    public void HardReset()
    {
        DataController.Instance.ResetAllData();
        _ballSpawnCounter.ResetCount();
    }
}
