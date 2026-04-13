using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class TutorialStep
{
    public string stepId;

    [Header("UI")]
    [Tooltip("Current tutorial required UI panel")]
    public GameObject pageUI;

    [Tooltip("Player must press the correct input (e.g. grab), either empty or many")]
    public InputActionReference[] requiredActions;

    [Header("When step start")]
    [Tooltip("When step start will trigger these event, any play sound event need to put here")]
    public UnityEvent onStepStart;

    [Tooltip("After achieve the target, these event will be called, only put the logic function here, don't put sound event")]
    public UnityEvent onStepComplete;
}

public class VRTutorialManager : MonoBehaviour
{
    [Header("Tutorial step config (follow the sequence)")]
    public TutorialStep[] steps;

    public int currentIndex = 0;

    // prevent player press the key in same time
    private bool isStepCompleting = false;

    public bool isDebug = false;
    private void Start()
    {
        if (steps == null || steps.Length == 0) return;

        foreach (var step in steps)
        {
            if (step.pageUI != null) step.pageUI.SetActive(false);
        }

        StartStep(0);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && isDebug)
            PassCurrentStep(steps[currentIndex].stepId);
#endif
    }

    private void StartStep(int index)
    {
        if (index >= steps.Length)
        {
            Debug.Log("<color=green>Tutorial steps completed!</color>");
            gameObject.SetActive(false);
            return;
        }

        currentIndex = index;
        isStepCompleting = false;
        TutorialStep currentStep = steps[currentIndex];

        if (currentStep.pageUI != null)
        {
            currentStep.pageUI.SetActive(true);
        }

        currentStep.onStepStart?.Invoke();

        if (currentStep.requiredActions != null && currentStep.requiredActions.Length > 0)
        {
            foreach (var actionRef in currentStep.requiredActions)
            {
                if (actionRef != null && actionRef.action != null)
                {
                    actionRef.action.Enable();
                    actionRef.action.performed += OnRequiredActionPerformed;
                }
            }
        }
    }

    private void OnRequiredActionPerformed(InputAction.CallbackContext context)
    {
        PassCurrentStep(steps[currentIndex].stepId);
    }

    // passedId must same with the TutorialStep config
    public void PassCurrentStep(string passedId)
    {
        if (isStepCompleting) return;

        TutorialStep currentStep = steps[currentIndex];

        if (passedId != currentStep.stepId)
        {
            Debug.LogWarning($"<color=orange>An illegal trigger was blocked! An ID: [{currentStep.stepId}] is required at this step，but " +
                $"received illegal ID: [{passedId}]. Ignored</color>");
            return;
        }

        isStepCompleting = true;

        // Clear old listener
        if (currentStep.requiredActions != null)
        {
            foreach (var actionRef in currentStep.requiredActions)
            {
                if (actionRef != null && actionRef.action != null)
                {
                    actionRef.action.performed -= OnRequiredActionPerformed;
                }
            }
        }

        currentStep.onStepComplete?.Invoke();

        if (currentStep.pageUI != null)
        {
            currentStep.pageUI.SetActive(false);
        }

        // Go to next step
        StartStep(currentIndex + 1);
    }

    private void OnDisable()
    {
        // Prevent script accidentally disable cause memory leak
        if (currentIndex < steps.Length)
        {
            var actions = steps[currentIndex].requiredActions;
            if (actions != null)
            {
                foreach (var actionRef in actions)
                {
                    if (actionRef != null && actionRef.action != null)
                    {
                        actionRef.action.performed -= OnRequiredActionPerformed;
                    }
                }
            }
        }
    }
}