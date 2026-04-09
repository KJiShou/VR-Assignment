using UnityEngine;
using UnityEngine.SceneManagement;

public enum Stage
{
    MainMenu,
    Tutorial,
    Stage1,
    Stage2,
}

[System.Serializable]
public struct StageData
{
    public string stage;
    public AudioClip bgm;
}
/// <summary>
/// Load scene using name, or reload the active scene
/// </summary>
public class LoadScene : MonoBehaviour
{
    public StageData[] stages;

    public void LoadSceneUsingName(string sceneName)
    {
        if(AudioManager.Instance != null)
        {
            foreach (var item in stages)
            {
                if (item.bgm != null && item.stage == sceneName)
                {
                    AudioManager.Instance.PlayAmbientSound(item.bgm);
                }
            }
        }

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
