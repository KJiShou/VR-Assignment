using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI masterVolumeText;
    [SerializeField] Slider masterSlider;
    [SerializeField] TextMeshProUGUI sfxVolumeText;
    [SerializeField] Slider bgmSlider;
    [SerializeField] TextMeshProUGUI ambientVolumeText;
    [SerializeField] Slider sfxSlider;

    private void Start()
    {
        masterSlider.value = AudioManager.Instance.GetSavedMasterVolume();
        bgmSlider.value = AudioManager.Instance.GetSavedBGMVolume();
        sfxSlider.value = AudioManager.Instance.GetSavedSFXVolume();
    }

    public void OnMasterVolumeChanged(float volume)
    {
        masterVolumeText.text = $"{(volume * 100):F0}%";
    }

    public void OnSFXVolumeChanged(float volume)
    {
        sfxVolumeText.text = $"{(volume * 100):F0}%";
    }

    public void OnAmbientVolumeChanged(float volume)
    {
        ambientVolumeText.text = $"{(volume * 100):F0}%";
    }
}
