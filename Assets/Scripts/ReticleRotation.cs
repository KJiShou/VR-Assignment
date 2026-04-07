using UnityEngine;

public class ReticleRotation : MonoBehaviour
{
    public float rotationSpeed = 180f;

    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.Self);
    }
}
