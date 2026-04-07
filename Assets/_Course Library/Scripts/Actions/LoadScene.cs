using UnityEngine;
using UnityEngine.SceneManagement;

public enum Stage
{
    MainMenu,
    Tutorial,
    Stage1,
    Stage2,
}

/// <summary>
/// Load scene using name, or reload the active scene
/// </summary>
public class LoadScene : MonoBehaviour
{
    private Stage stage;
    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == (int)Stage.Tutorial)
        {
            //AudioManager.Instance.
        }
    }

    public void LoadSceneUsingName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void LoadNextScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync((currentScene.buildIndex + 1) % SceneManager.sceneCount);
    }
}
