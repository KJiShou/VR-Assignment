using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentLevel;
    private const string PREF_CURRENT_LEVEL = "CurrentLevel_Pref";

    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt(PREF_CURRENT_LEVEL, 0);
    }

    public void StageCleared()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentLevel < currentSceneIndex)
        {
            PlayerPrefs.SetInt(PREF_CURRENT_LEVEL, currentSceneIndex);

            PlayerPrefs.Save();

            currentLevel = currentSceneIndex;

            Debug.Log($"<color=green>Progress Saved! Highest Level: {currentLevel}</color>");
        }
    }
}
