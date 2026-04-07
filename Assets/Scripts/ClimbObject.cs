using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRBaseInteractable))]
public class ClimbObject : MonoBehaviour
{
    public float reducedStamina = 1.0f;

    private XRBaseInteractable _interactable;
    private StaminaManager staminaManager;

    private IXRSelectInteractor leftHandInteractor;
    private IXRSelectInteractor rightHandInteractor;

    private SimpleAudioFeedback simpleAudioFeedback;

    void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();
        staminaManager = FindFirstObjectByType<StaminaManager>();

        _interactable.selectEntered.AddListener(OnGrab);
        _interactable.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        if (AudioManager.Instance.sfxSource != null)
        {
            if (TryGetComponent<SimpleAudioFeedback>(out simpleAudioFeedback))
            {
                simpleAudioFeedback.audioSource = AudioManager.Instance.sfxSource;
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;

        if (interactor.transform.CompareTag("RightHand"))
        {
            rightHandInteractor = interactor;
            staminaManager.StartHoldingRight(reducedStamina, this);
        }
        else if (interactor.transform.CompareTag("LeftHand"))
        {
            leftHandInteractor = interactor;
            staminaManager.StartHoldingLeft(reducedStamina, this);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject;

        if (interactor.transform.CompareTag("RightHand"))
        {
            if (rightHandInteractor == interactor)
                rightHandInteractor = null;

            staminaManager.StopHoldingRight(this);
        }
        else if (interactor.transform.CompareTag("LeftHand"))
        {
            if (leftHandInteractor == interactor)
                leftHandInteractor = null;

            staminaManager.StopHoldingLeft(this);
        }
    }

    public void ForceReleaseLeftHand()
    {
        if (leftHandInteractor != null && _interactable.interactionManager != null)
        {
            _interactable.interactionManager.SelectExit(leftHandInteractor, _interactable);
            leftHandInteractor = null;
        }
    }

    public void ForceReleaseRightHand()
    {
        if (rightHandInteractor != null && _interactable.interactionManager != null)
        {
            _interactable.interactionManager.SelectExit(rightHandInteractor, _interactable);
            rightHandInteractor = null;
        }
    }

    void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnGrab);
            _interactable.selectExited.RemoveListener(OnRelease);
        }
    }
}