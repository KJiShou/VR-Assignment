using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public ClimbDataLogger dataManager;
    List<ClimbRecord> currentMapHistory = new List<ClimbRecord>();

    [SerializeField] GameObject rankPrefab;
    [SerializeField] Transform prefabParent;

    [SerializeField] Material firstRankMat;
    [SerializeField] Material secondRankMat;
    [SerializeField] Material thirdRankMat;

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        currentMapHistory = dataManager.LoadMapRecords(sceneName);

        // OrderBy: Ascending Order (Shorter time, higher rank)
        // Take(5): Only get top 5
        var topFiveRecords = currentMapHistory
            .OrderBy(record => record.TimeSpent)
            .Take(5)
            .ToList();

        int i = 1;
        foreach (ClimbRecord record in topFiveRecords)
        {
            GameObject rank = Instantiate(rankPrefab, prefabParent);

            var rankText = rank.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            var usedTime = rank.transform.Find("Used Time").GetComponent<TextMeshProUGUI>();
            var dateTime = rank.transform.Find("DateTime").GetComponent<TextMeshProUGUI>();

            rankText.text = $"#{i}";
            usedTime.text = record.TimeSpent;
            dateTime.text = $"{record.Date} {record.Timestamp}";

            switch (i) 
            {
                case 1:
                    rank.GetComponent<Image>().material = firstRankMat;
                    break;
                case 2:
                    rank.GetComponent<Image>().material = secondRankMat;
                    break;
                case 3:
                    rank.GetComponent<Image>().material = thirdRankMat;
                    break;
                default:
                    rank.GetComponent<Image>().material = null;
                    break;
            }

            i++;
        }
    }
}