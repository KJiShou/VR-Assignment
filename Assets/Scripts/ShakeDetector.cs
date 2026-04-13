using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShakeDetector : MonoBehaviour
{
    public XRDirectInteractor directInteractor;

    [Header("Input binding")]
    [Tooltip("Device velocity")]
    public InputActionReference velocityAction;

    [Header("Shake parameters")]
    [Tooltip("higher value harder to trigger")]
    public float shakeThreshold = 2.5f;

    [Tooltip("Trigger event interval time")]
    public float cooldownTime = 0.5f;

    [Header("Trigger event")]
    [Tooltip("What action should do after shaking")]
    public UnityEvent onShakeTriggered;

    public bool isDebug = false;

    private float _lastShakeTime = 0f;

    private void OnEnable()
    {
        if (velocityAction != null)
            velocityAction.action.Enable();
    }

    private void OnDisable()
    {
        if (velocityAction != null)
            velocityAction.action.Disable();
    }

    private void Update()
    {
        if (velocityAction == null) return;

        bool canShake = true;

        if (directInteractor != null && directInteractor.hasSelection)
        {
            var grabbedInteractable = directInteractor.firstInteractableSelected;

            if (grabbedInteractable != null && grabbedInteractable.transform.TryGetComponent<ClimbObject>(out _))
            {
                canShake = false;
            }
            else
            {
                canShake = true;
            }
        }

#if UNITY_EDITOR
        // Pressing space directly trigger shake action, for testing purpose 
        if (Keyboard.current != null && isDebug && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Time.time >= _lastShakeTime + cooldownTime)
            {
                TriggerShakeAction(999f);
                return;
            }
        }
#endif

        if(canShake)
        {
            Vector3 currentVelocity = velocityAction.action.ReadValue<Vector3>();

            float speed = currentVelocity.magnitude;

            if (speed >= shakeThreshold && Time.time >= _lastShakeTime + cooldownTime)
            {
                TriggerShakeAction(speed);
            }
        }
    }

    private void TriggerShakeAction(float currentSpeed)
    {
        _lastShakeTime = Time.time;

        Debug.Log($"<color=cyan>[Shake Detection]</color> Trigger shake! Current speed: {currentSpeed}");

        onShakeTriggered?.Invoke();
    }
}