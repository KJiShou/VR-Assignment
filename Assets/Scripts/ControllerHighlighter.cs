using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerHighlighter : MonoBehaviour
{
    private GameObject leftXButtonHighlight;
    private GameObject rightAButtonHighlight;
    private GameObject leftGripHighlight;
    private GameObject rightGripHighlight;
    private GameObject leftMenuButtonHighlight;
    private GameObject leftThumbStickHighlight;
    private GameObject rightThumbStickHighlight;

    private HashSet<GameObject> activeHighlights = new HashSet<GameObject>();
    private Coroutine masterFlashRoutine;

    [Tooltip("Time foe switching highlight from inactive to active")]
    public float activeInterval = 0.2f;

    [Tooltip("Time foe switching highlight from active to inactive")]
    public float inactiveInterval = 0.4f;

    public void FlashLeftX() => StartCoroutine(WaitAndFlashRoutine("Left_X_Button_Highlight", obj => leftXButtonHighlight = obj));
    public void FlashRightA() => StartCoroutine(WaitAndFlashRoutine("Right_A_Button_Highlight", obj => rightAButtonHighlight = obj));
    public void FlashLeftGrip() => StartCoroutine(WaitAndFlashRoutine("Left_Grip_Highlight", obj => leftGripHighlight = obj));
    public void FlashRightGrip() => StartCoroutine(WaitAndFlashRoutine("Right_Grip_Highlight", obj => rightGripHighlight = obj));
    public void FlashLeftMenu() => StartCoroutine(WaitAndFlashRoutine("Left_Menu_Button_Highlight", obj => leftMenuButtonHighlight = obj));
    public void FlashLeftThumbStick() => StartCoroutine(WaitAndFlashRoutine("Left_ThumbStick_Highlight", obj => leftThumbStickHighlight = obj));
    public void FlashRightThumbStick() => StartCoroutine(WaitAndFlashRoutine("Right_ThumbStick_Highlight", obj => rightThumbStickHighlight = obj));

    public void FlashBothGrips()
    {
        FlashLeftGrip();
        FlashRightGrip();
    }

    public void FlashXAndAButtons()
    {
        FlashLeftX();
        FlashRightA();
    }

    // Must attach this script at the parent of the objects that you want to find
    private GameObject FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child.gameObject;
            GameObject result = FindDeepChild(child, childName);
            if (result != null) return result;
        }
        return null;
    }

    public void StopAllFlashing()
    {
        if (masterFlashRoutine != null)
        {
            StopCoroutine(masterFlashRoutine);
            masterFlashRoutine = null;
        }

        foreach (var btn in activeHighlights)
        {
            if (btn != null) btn.SetActive(false);
        }
        activeHighlights.Clear();

        if (leftXButtonHighlight) leftXButtonHighlight.SetActive(false);
        if (rightAButtonHighlight) rightAButtonHighlight.SetActive(false);
        if (leftGripHighlight) leftGripHighlight.SetActive(false);
        if (rightGripHighlight) rightGripHighlight.SetActive(false);
        if (leftMenuButtonHighlight) leftMenuButtonHighlight.SetActive(false);
        if (leftThumbStickHighlight) leftThumbStickHighlight.SetActive(false);
        if (rightThumbStickHighlight) rightThumbStickHighlight.SetActive(false);
    }

    // ==========================================
    // Asynchronous Item Finding Coroutine with Timeout Protection
    // ==========================================
    private IEnumerator WaitAndFlashRoutine(string buttonName, System.Action<GameObject> saveFoundObject)
    {
        GameObject foundObj = null;
        float timeout = 3.0f; // Maximum wait for 3 sec
        float timer = 0f;

        // time not reach time out, then keep finding
        while (timer < timeout)
        {
            foundObj = FindDeepChild(this.transform, buttonName);

            if (foundObj != null)
            {
                break; // if found, directly break the loop
            }

            timer += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }

        if (foundObj == null)
        {
            Debug.LogError($"<color=red>[Time Out Error] Already wait for {timeout} sec, model still haven't instantiate! Can't found {buttonName} !" +
                $" Please check the prefab is correctly loaded or not!</color>");
            yield break; // End coroutine
        }

        // Assign value to the variable
        saveFoundObject?.Invoke(foundObj);

        activeHighlights.Add(foundObj);

        if (masterFlashRoutine == null)
        {
            masterFlashRoutine = StartCoroutine(MasterFlashRoutine());
        }
    }

    private IEnumerator MasterFlashRoutine()
    {
        while (activeHighlights.Count > 0)
        {
            foreach (var btn in activeHighlights) if (btn != null) btn.SetActive(true);
            yield return new WaitForSeconds(inactiveInterval);

            foreach (var btn in activeHighlights) if (btn != null) btn.SetActive(false);
            yield return new WaitForSeconds(activeInterval);
        }
        masterFlashRoutine = null;
    }
}