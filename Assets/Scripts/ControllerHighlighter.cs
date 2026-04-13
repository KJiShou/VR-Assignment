using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerHighlighter : MonoBehaviour
{
    [SerializeField] GameObject leftXButtonHighlight;
    [SerializeField] GameObject rightAButtonHighlight;
    [SerializeField] GameObject leftGripHighlight;
    [SerializeField] GameObject rightGripHighlight;
    [SerializeField] GameObject leftMenuButtonHighlight;
    [SerializeField] GameObject leftThumbStickHighlight;
    [SerializeField] GameObject rightThumbStickHighlight;

    private HashSet<GameObject> activeHighlights = new HashSet<GameObject>();
    private Coroutine masterFlashRoutine;

    [Tooltip("Time foe switching highlight from inactive to active")]
    public float activeInterval = 0.2f;

    [Tooltip("Time foe switching highlight from active to inactive")]
    public float inactiveInterval = 0.4f;

    //public void FlashLeftX() => StartCoroutine(WaitAndFlashRoutine("Left_X_Button_Highlight", obj => leftXButtonHighlight = obj));
    //public void FlashRightA() => StartCoroutine(WaitAndFlashRoutine("Right_A_Button_Highlight", obj => rightAButtonHighlight = obj));
    //public void FlashLeftGrip() => StartCoroutine(WaitAndFlashRoutine("Left_Grip_Highlight", obj => leftGripHighlight = obj));
    //public void FlashRightGrip() => StartCoroutine(WaitAndFlashRoutine("Right_Grip_Highlight", obj => rightGripHighlight = obj));
    //public void FlashLeftMenu() => StartCoroutine(WaitAndFlashRoutine("Left_Menu_Button_Highlight", obj => leftMenuButtonHighlight = obj));
    //public void FlashLeftThumbStick() => StartCoroutine(WaitAndFlashRoutine("Left_ThumbStick_Highlight", obj => leftThumbStickHighlight = obj));
    //public void FlashRightThumbStick() => StartCoroutine(WaitAndFlashRoutine("Right_ThumbStick_Highlight", obj => rightThumbStickHighlight = obj));

    public void FlashLeftX() => FlushRoutine(leftXButtonHighlight);
    public void FlashRightA() => FlushRoutine(rightAButtonHighlight);
    public void FlashLeftGrip() => FlushRoutine(leftGripHighlight);
    public void FlashRightGrip() => FlushRoutine(rightGripHighlight);
    public void FlashLeftMenu() => FlushRoutine(leftMenuButtonHighlight);
    public void FlashLeftThumbStick() => FlushRoutine(leftThumbStickHighlight);
    public void FlashRightThumbStick() => FlushRoutine(rightThumbStickHighlight);

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

    private void FlushRoutine(GameObject obj)
    {
        activeHighlights.Add(obj);

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