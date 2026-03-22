using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Player : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TopPoint")
        {
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = true;
                rightRayInteractor.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TopPoint")
        {
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = false;
                rightRayInteractor.enabled = false;
            }
        }
    }
}
