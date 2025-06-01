using UnityEngine;

public class LoadScene : MonoBehaviour
{
    private void Awake()
    {
        UpgradeManager.Instance.OnInitialized += LoadMainScene;
    }
    private void LoadMainScene()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main Scene").isLoaded)
        {
            Debug.Log("Main scene already loaded");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        Debug.Log("Main scene loaded");
    }
}
