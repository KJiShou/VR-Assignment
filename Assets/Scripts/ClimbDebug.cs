using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ClimbDebug : MonoBehaviour
{
    private XRBaseInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();

        _interactable.selectEntered.AddListener(OnGrab);
        _interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"<color=green>[CLIMB]</color> Grab! Controller: <b>{args.interactorObject.transform.name}</b> catch <b>{gameObject.name}</b>");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log($"<color=red>[CLIMB]</color> Released. Controller: <b>{args.interactorObject.transform.name}</b> leaves <b>{gameObject.name}</b>");
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
