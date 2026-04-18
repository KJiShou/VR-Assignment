using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PaintDrawing : MonoBehaviour
{
    [Header("Core References")]
    [Tooltip("Drag paint brush object components")]
    public XRGrabInteractable grabInteractable;
    public GameObject linePrefab;
    public LayerMask canvasLayer;

    [Header("Paint Brush Settings")]
    public float lineWidth = 0.01f;
    public float minDistance = 0.005f;

    [Tooltip("XR Rig Main Camera")]
    public Transform playerCamera;

    public PlayContinuousSound playContinuousSound;

    public float frontFloating = 0.002f;
    public float backFloating = 0.004f;

    public Color color = Color.white;

    [Header("UI References")]
    [Tooltip("Color palette canvas or its parent object")]
    public GameObject colorPaletteUI;
    [Tooltip("Choose trigger button")]
    public InputAction secondaryButtonInput;
    public GameObject[] othersUI;

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();

    private bool isTriggerPressed = false; 
    private Transform currentCanvas;   
    private Collider currentCanvasCollider;

    private Vector3 lastPointWorldPos;

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(OnTriggerPulled);
            grabInteractable.deactivated.AddListener(OnTriggerReleased);
            grabInteractable.selectExited.AddListener(OnSelectExcited);
        }
        else
        {
            Debug.LogError("Please assign paint brush XRGrabInteractable to PaintDrawing.cs");
        }

        if (secondaryButtonInput != null)
        {
            secondaryButtonInput.Enable();

            // OnPressed
            secondaryButtonInput.performed += OnSecondaryButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnTriggerPulled);
            grabInteractable.deactivated.RemoveListener(OnTriggerReleased);
            grabInteractable.selectExited.RemoveListener(OnSelectExcited);
        }
        if (secondaryButtonInput != null)
        {
            secondaryButtonInput.performed -= OnSecondaryButtonPressed;

            secondaryButtonInput.Disable();
        }
    }

    private void OnTriggerPulled(ActivateEventArgs args)
    {
        isTriggerPressed = true;
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        isTriggerPressed = false;
        EndTrail();
    }

    private void OnSelectExcited(SelectExitEventArgs args)
    {
        CloseColorPalette();
    }

    private void OnSecondaryButtonPressed(InputAction.CallbackContext context)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            bool isCurrentlyActive = colorPaletteUI.activeSelf;
            colorPaletteUI.SetActive(!isCurrentlyActive);

            foreach (var item in othersUI)
            {
                if (item.activeSelf && !isCurrentlyActive) item.SetActive(false);
            }
        }
    }

    public void CloseColorPalette()
    {
        if(colorPaletteUI.activeSelf) colorPaletteUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & canvasLayer) != 0)
        {
            currentCanvas = other.transform;
            currentCanvasCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentCanvas != null && other.transform == currentCanvas)
        {
            currentCanvas = null;
            currentCanvasCollider = null;
            EndTrail();
        }
    }

    private void Update()
    {
        // If pressing trigger AND pen tip touching canvas
        if (isTriggerPressed && currentCanvas != null && currentCanvasCollider != null)
        {
            // Is drawing at front of canvas
            if (IsCameraOnFrontSide())
            {
                if (currentLine == null)
                {
                    StartTrail();
                }
                AddPoint(frontFloating);
            }
            else
            {
                if (currentLine == null)
                {
                    StartTrail();
                }
                AddPoint(backFloating);
            }
            if (!playContinuousSound.audioSource.isPlaying)
            {
                playContinuousSound.Play();
            }
        }
    }

    private bool IsCameraOnFrontSide()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Haven't assign Player Camera!");
            return true;
        }

        // Canvas to camera direction
        Vector3 directionToCamera = playerCamera.position - currentCanvas.position;

        // Canvas front vector
        Vector3 canvasForward = currentCanvas.up;

        return Vector3.Dot(canvasForward, directionToCamera) > -0.01f;
    }

    private bool IsPenOnFrontSide()
    {
        Vector3 directionToPen = transform.position - currentCanvas.position;

        Vector3 canvasForward = currentCanvas.up;

        float dotProduct = Vector3.Dot(canvasForward, directionToPen);

        return dotProduct > -0.01f;
    }

    private void StartTrail()
    {
        GameObject lineObj = Instantiate(linePrefab, currentCanvas);
        currentLine = lineObj.GetComponent<LineRenderer>();
        currentLine.useWorldSpace = false;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.startColor = color;
        currentLine.endColor = color;
        points.Clear();
        lastPointWorldPos = Vector3.zero;
    }

    private void AddPoint(float offset)
    {
        Vector3 canvasNormal = currentCanvas.up;

        Plane canvasPlane = new Plane(canvasNormal, currentCanvas.position);

        // Stick the pen tip coordinate on the plane
        Vector3 surfacePoint = canvasPlane.ClosestPointOnPlane(transform.position);

        // Prevent Z-fighting
        Vector3 offsetPoint = surfacePoint + (canvasNormal * offset);

        Vector3 localPos = currentCanvas.InverseTransformPoint(offsetPoint);

        if (points.Count == 0 || Vector3.Distance(lastPointWorldPos, offsetPoint) > minDistance)
        {
            points.Add(localPos);
            currentLine.positionCount = points.Count;
            currentLine.SetPosition(points.Count - 1, localPos);

            lastPointWorldPos = offsetPoint;
        }
    }

    private void EndTrail()
    {
        currentLine = null;
        if (playContinuousSound.audioSource.isPlaying)
        {
            playContinuousSound.Pause();
        }
    }
}