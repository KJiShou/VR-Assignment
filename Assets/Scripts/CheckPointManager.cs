using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Tooltip("Put the checkpoints in sequence (index 0 = first checkpoint, index 1 = second checkpoint, ...)")]
    public List<CheckPoint> checkPoints = new List<CheckPoint>();

    public int currentCheckPointIndex = -1;

    void Awake()
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if (checkPoints[i] != null)
            {
                checkPoints[i].Setup(this, i);
            }
        }
    }

    public void ActivateCheckPoint(int index)
    {
        // Only the reach new check point will update checkpoint index
        if (index > currentCheckPointIndex)
        {
            currentCheckPointIndex = index;
            Debug.Log($"<color=green>[CHECKPOINT]</color> Progress saved! Current checkpoint index: {index}");
        }
    }
}
