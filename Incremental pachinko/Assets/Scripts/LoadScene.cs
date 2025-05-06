using UnityEngine;

public class LoadScene : MonoBehaviour
{

    // load main scene when UpgradeManager is initialized
    private void Awake()
    {
        UpgradeManager.Instance.OnInitialized += LoadMainScene;
    }
    private void LoadMainScene()
    {
        // load the main scene here if scene is not already loaded
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main Scene").isLoaded)
        {
            Debug.Log("Main scene already loaded");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        Debug.Log("Main scene loaded");
    }
}
