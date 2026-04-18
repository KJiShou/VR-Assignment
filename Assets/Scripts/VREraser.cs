using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VREraser : MonoBehaviour
{
    [Header("Eraser Settings")]
    public Collider eraserCollider;
    public GameObject linePrefab;

    [Header("Input and controls")]
    public XRGrabInteractable grabInteractable;
    [Tooltip("Drawing canvas")]
    public Transform currentCanvas;

    private PlayContinuousSound playContinuousSound;

    private bool isErasing = false;

    private void Awake()
    {
        playContinuousSound = GetComponent<PlayContinuousSound>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(StartErase);
            grabInteractable.deactivated.AddListener(StopErase);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(StartErase);
            grabInteractable.deactivated.RemoveListener(StopErase);
        }
    }

    private void StartErase(ActivateEventArgs args) { isErasing = true; }
    private void StopErase(DeactivateEventArgs args) {
        if (playContinuousSound.IsPlaying())
        {
            playContinuousSound.Pause();
        }

        isErasing = false; 
    }

    private void Update()
    {
        if (isErasing && currentCanvas != null && eraserCollider != null)
        {
            EraseLinesWithCollider();
        }
    }

    // ==========================================
    // Use collider to determine erase position
    // ==========================================
    private void EraseLinesWithCollider()
    {
        LineRenderer[] allLines = currentCanvas.GetComponentsInChildren<LineRenderer>();

        foreach (LineRenderer line in allLines)
        {
            int pointCount = line.positionCount;
            if (pointCount == 0) continue;

            Vector3[] localPoints = new Vector3[pointCount];
            line.GetPositions(localPoints);

            List<List<Vector3>> splitLineSegments = new List<List<Vector3>>();
            List<Vector3> currentSegment = new List<Vector3>();
            bool lineWasModified = false;

            for (int i = 0; i < pointCount; i++)
            {
                // Convert local pos to global pos
                Vector3 pointWorldPos = line.transform.TransformPoint(localPoints[i]);

                // Determine the point is it in the collider
                Vector3 closestPoint = eraserCollider.ClosestPoint(pointWorldPos);

                bool isInsideCollider = Vector3.Distance(closestPoint, pointWorldPos) < 0.001f;

                if (isInsideCollider)
                {
                    // This point is erase
                    lineWasModified = true;

                    if (currentSegment.Count >= 2)
                    {
                        splitLineSegments.Add(new List<Vector3>(currentSegment));
                    }
                    currentSegment.Clear(); // Break point
                }
                else
                {
                    // Remain the point
                    currentSegment.Add(localPoints[i]);
                }
            }

            if (currentSegment.Count >= 2)
            {
                splitLineSegments.Add(currentSegment);
            }

            if (lineWasModified)
            {
                float oldWidth = line.startWidth;
                Color startColor = line.startColor;
                Color endColor = line.endColor;

                Destroy(line.gameObject);

                foreach (List<Vector3> segment in splitLineSegments)
                {
                    CreateNewLine(segment, oldWidth, startColor, endColor);
                }
            }
        }
    }

    // ==========================================
    // Use radius to determine erase position
    // ==========================================
    private void EraseLinesNearPosition(Vector3 eraserWorldPos, float radius)
    {
        LineRenderer[] allLines = currentCanvas.GetComponentsInChildren<LineRenderer>();

        foreach (LineRenderer line in allLines)
        {
            int pointCount = line.positionCount;
            if (pointCount == 0) continue;

            Vector3[] localPoints = new Vector3[pointCount];
            line.GetPositions(localPoints);

            List<List<Vector3>> splitLineSegments = new List<List<Vector3>>();
            List<Vector3> currentSegment = new List<Vector3>();

            bool lineWasModified = false;

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 pointWorldPos = line.transform.TransformPoint(localPoints[i]);
                float distance = Vector3.Distance(pointWorldPos, eraserWorldPos);

                if (distance <= radius)
                {
                    lineWasModified = true;

                    // If the currently accumulated line segment has at least 2 points, it is a valid line; save it.
                    if (currentSegment.Count >= 2)
                    {
                        splitLineSegments.Add(new List<Vector3>(currentSegment));
                    }
                    currentSegment.Clear();
                }
                else
                {
                    currentSegment.Add(localPoints[i]);
                }
            }

            if (currentSegment.Count >= 2)
            {
                splitLineSegments.Add(currentSegment);
            }

            if (lineWasModified)
            {
                float oldWidth = line.startWidth;
                Color startColor = line.startColor;
                Color endColor = line.endColor;

                Destroy(line.gameObject);

                foreach (List<Vector3> segment in splitLineSegments)
                {
                    CreateNewLine(segment, oldWidth, startColor, endColor);
                }
            }
        }
    }

    // Generate entirely new lines based on the surviving points.
    private void CreateNewLine(List<Vector3> points, float width, Color startColor, Color endColor)
    {
        GameObject newLineObj = Instantiate(linePrefab, currentCanvas);
        LineRenderer lr = newLineObj.GetComponent<LineRenderer>();

        lr.useWorldSpace = false;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = startColor;
        lr.endColor = endColor;

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());

        if (!playContinuousSound.IsPlaying())
        {
            playContinuousSound.Play();
        }
    }
}