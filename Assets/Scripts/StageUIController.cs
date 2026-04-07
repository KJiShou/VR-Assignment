using UnityEngine;
using UnityEngine.UI;

public class StageUIController : MonoBehaviour
{
    private int currentLevel;
    private const string PREF_CURRENT_LEVEL = "CurrentLevel_Pref";

    [SerializeField] GameObject[] stageUis;
    private Button[] stageButtons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(PREF_CURRENT_LEVEL, 0);
        stageButtons = new Button[stageUis.Length];
        for (int i = 0; i < stageUis.Length; i++)
        {
            if (stageUis[i].TryGetComponent<Button>(out stageButtons[i]))
            {
                bool isUnlocked = (currentLevel > i);

                stageButtons[i].interactable = isUnlocked;
                stageUis[i].transform.GetChild(1).gameObject.SetActive(!isUnlocked);
            }
        }
    }
}
