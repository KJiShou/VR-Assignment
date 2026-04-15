using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GrabbableObject : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip sfx;
    private Rigidbody rb;
    private bool firstPlay = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb != null && audioSource != null && !firstPlay)
        {
            Debug.Log($"Drop on floor by this speed{rb.linearVelocity.magnitude:F2}");
            // audioSource.PlayOneShot(sfx, rb.linearVelocity.magnitude * 10);
            audioSource.PlayOneShot(sfx);
        }
        firstPlay = false;
    }
}