using UnityEngine;

[RequireComponent(typeof(Material))]
public class Flag : MonoBehaviour
{
    private Material material;

    [Range(0.5f, 10.0f)]
    [SerializeField] private float waveIntensity = 1.0f;

    [Range(0.5f, 10.0f)]
    [SerializeField] private float waveSpeed = 4.0f;

    private float stopWaveIntensity = 0.5f;
    private float stopWaveSpeed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = this.GetComponent<Material>();
    }

    public void SetFlagWave()
    {
        material.SetFloat("_Wave_Intensity", waveIntensity);
        material.SetFloat("_Wave_Speed", waveSpeed);
    }

    public void StopFlagWave()
    {
        material.SetFloat("_Wave_Intensity", stopWaveIntensity);
        material.SetFloat("_Wave_Speed", stopWaveSpeed);
    }
}
