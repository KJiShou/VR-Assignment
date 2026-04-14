using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[System.Serializable]
public struct ClimbRecord
{
    public string MapName;    
    public string TimeSpent;  
    public string Date;       
    public string Timestamp;  
}
public class ClimbDataLogger : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "ClimbingRecords.csv");

        if (!File.Exists(filePath))
        {
            string header = "Stage,Used Time,Date,Time\n";
            File.WriteAllText(filePath, header, Encoding.UTF8);
        }
    }

    /// <summary>
    /// Call this function to record climbing time into CSV
    /// </summary>
    /// <param name="mapName">Scene Name</param>
    /// <param name="timeSpent">Climbing Time (e.g 00:45.22)</param>
    public void SaveClimbRecord(string mapName, string timeSpent)
    {
        string date = System.DateTime.Now.ToString("yyyy-MM-dd");
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");

        string newEntry = $"{mapName},{timeSpent},{date},{timestamp}\n";

        File.AppendAllText(filePath, newEntry, Encoding.UTF8);

        Debug.Log($"Data saved to Excel(CSV): {filePath}");
    }

    /// <summary>
    /// read local CSV file, return whole records as List<ClimbRecord>
    /// </summary>
    public List<ClimbRecord> LoadAllRecords()
    {
        List<ClimbRecord> recordsList = new List<ClimbRecord>();

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("<color=yellow>Can't found record file, player may not create any record</color>");
            return recordsList;
        }

        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

        // Start from row 1 since row 0 is header (Stage,Used Time,Date,Time)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            // Empty row
            if (string.IsNullOrWhiteSpace(line)) continue;

            // ',' is seperator
            string[] rawData = line.Split(',');

            // Make sure have 4 pieces of data, prevent data corrupt
            if (rawData.Length >= 4)
            {
                ClimbRecord entry = new ClimbRecord();

                entry.MapName = rawData[0];
                entry.TimeSpent = rawData[1];
                entry.Date = rawData[2];
                entry.Timestamp = rawData[3];

                recordsList.Add(entry);
            }
        }

        Debug.Log($"<color=green>Successfully read {recordsList.Count} of climbing record!</color>");
        return recordsList;
    }

    /// <summary>
    /// read local CSV file, return specific map record
    /// </summary>
    public List<ClimbRecord> LoadMapRecords(string mapName)
    {
        List<ClimbRecord> recordsList = new List<ClimbRecord>();

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("<color=yellow>Can't found record file, player may not create any record</color>");
            return recordsList;
        }

        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

        // Start from row 1 since row 0 is header (Stage,Used Time,Date,Time)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            // Empty row
            if (string.IsNullOrWhiteSpace(line)) continue;

            // ',' is seperator
            string[] rawData = line.Split(',');

            // Make sure have 4 pieces of data, prevent data corrupt
            if (rawData.Length >= 4 && rawData[0] == mapName)
            {
                ClimbRecord entry = new ClimbRecord();

                entry.MapName = rawData[0];
                entry.TimeSpent = rawData[1];
                entry.Date = rawData[2];
                entry.Timestamp = rawData[3];

                recordsList.Add(entry);
            }
        }

        Debug.Log($"<color=green>Successfully read {recordsList.Count} of climbing record for the map {mapName}!</color>");
        return recordsList;
    }
}
