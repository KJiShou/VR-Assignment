using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI masterVolumeText;
    [SerializeField] Slider masterSlider;
    [SerializeField] TextMeshProUGUI sfxVolumeText;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TextMeshProUGUI ambientVolumeText;
    [SerializeField] Slider ambientSlider;

    [SerializeField] GameManager gameManager;

    private void Start()
    {
        if (AudioManager.Instance == null) return;
        masterSlider.value = AudioManager.Instance.GetSavedMasterVolume();
        ambientSlider.value = AudioManager.Instance.GetSavedAmbientVolume();
        sfxSlider.value = AudioManager.Instance.GetSavedSFXVolume();

        //masterVolumeText.text = $"{(masterSlider.value * 100):F0}%";
        //sfxVolumeText.text = $"{(masterSlider.value * 100):F0}%";
        //ambientVolumeText.text = $"{(masterSlider.value * 100):F0}%";
    }

    public void OnMasterVolumeChanged(float volume)
    {
        if (AudioManager.Instance == null) return;
        masterVolumeText.text = $"{(volume * 100):F0}%";
        AudioManager.Instance.SetMasterVolume(volume);
    }

    public void OnSFXVolumeChanged(float volume)
    {
        if (AudioManager.Instance == null) return;
        sfxVolumeText.text = $"{(volume * 100):F0}%";
        AudioManager.Instance.SetSFXVolume(volume);
    }

    public void OnAmbientVolumeChanged(float volume)
    {
        if (AudioManager.Instance == null) return;
        ambientVolumeText.text = $"{(volume * 100):F0}%";
        AudioManager.Instance.SetAmbientVolume(volume);
    }
}
