using UnityEngine;

[RequireComponent(typeof(Material))]
public class Flag : MonoBehaviour
{
    [Range(0.5f, 10.0f)]
    [SerializeField] private float waveIntensity = 1.0f;

    [Range(0.5f, 10.0f)]
    [SerializeField] private float waveSpeed = 4.0f;

    private float stopWaveIntensity = 0.5f;
    private float stopWaveSpeed = 1.0f;

    public void SetFlagWave()
    {
        Shader.SetGlobalFloat("_Wave_Intensity", waveIntensity);
        Shader.SetGlobalFloat("_Wave_Speed", waveSpeed);
        Debug.Log($"<color=green>SetFlagWave()</color> Current wave intensity: {Shader.GetGlobalFloat("_Wave_Intensity")}");
        Debug.Log($"<color=green>SetFlagWave()</color> Current wave speed: {Shader.GetGlobalFloat("_Wave_Speed")}");
    }

    public void StopFlagWave()
    {
        Shader.SetGlobalFloat("_Wave_Intensity", stopWaveIntensity);
        Shader.SetGlobalFloat("_Wave_Speed", stopWaveSpeed);
        Debug.Log($"<color=green>StopFlagWave()</color> Current wave intensity: {Shader.GetGlobalFloat("_Wave_Intensity")}");
        Debug.Log($"<color=green>StopFlagWave()</color> Current wave speed: {Shader.GetGlobalFloat("_Wave_Speed")}");
    }
}
