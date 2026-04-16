using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ClimbingArea : MonoBehaviour
{
    [SerializeField] XRRayInteractor leftRayInteractor;
    [SerializeField] XRRayInteractor rightRayInteractor;

    public UnityEvent triggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEvent != null && other.CompareTag("Player")) 
        {
            triggerEvent.Invoke();
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = false;
                rightRayInteractor.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = true;
                rightRayInteractor.enabled = true;
            }
        }
    }
}
