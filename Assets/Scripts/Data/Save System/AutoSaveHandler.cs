using UnityEngine;

public class AutoSaveHandler : MonoBehaviour
{
    [SerializeField] private float _autoSaveInterval = 60f;
    private float _timer;
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _autoSaveInterval)
        {
            DataController.Instance.SaveData();
            _timer = 0f;
        }
    }
}
