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
        // load the main scene here
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        Debug.Log("Main scene loaded");
    }
}
