using UnityEngine;

public class AutoSaveHandler : MonoBehaviour
{
    public float autoSaveInterval = 60f;
    private float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= autoSaveInterval)
        {
            DataController.Instance.SaveData();
            timer = 0f;
        }
    }
}
