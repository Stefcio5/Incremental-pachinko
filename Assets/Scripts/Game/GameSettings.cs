using UnityEngine;

public class GameSettings : MonoBehaviour
{

    public void HardReset()
    {
        PlayerPrefs.DeleteAll();
        DataController.Instance.ResetAllData();
    }
}
