using UnityEngine;
using UnityEngine.Events;

public class ClimbingArea : MonoBehaviour
{
    public UnityEvent triggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEvent != null && other.CompareTag("Player")) 
        {
            triggerEvent.Invoke();
        }
    }
}
