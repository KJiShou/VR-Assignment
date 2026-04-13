using UnityEngine;
using TMPro;
using System;

public class ClimbTimer : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Used to show climbing time")]
    public TextMeshProUGUI watchTimeText;
    public TextMeshProUGUI winScreenTimeText;

    private float currentTime = 0f;
    public bool isTimerRunning { get; private set; } = false;

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartClimbingTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        Debug.Log("<color=cyan>Climbing timer start to count</color>");
    }

    public void StopClimbingTimer()
    {
        isTimerRunning = false;
        Debug.Log($"<color=cyan>Climbing timer stopped! Final use time: {GetFormattedTime()}</color>");
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        if (watchTimeText != null && winScreenTimeText != null)
        {
            string text = GetFormattedTime();
            watchTimeText.text = text;
            winScreenTimeText.text = "Used Time: " + text;
        }
    }

    /// <summary>
    /// Get the format time string
    /// </summary>
    /// <returns>time string 00:00.00</returns>
    private string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        return time.ToString(@"mm\:ss\.ff");
    }
}