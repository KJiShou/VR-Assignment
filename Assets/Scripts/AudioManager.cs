using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioMixer mainMixer;

    public AudioSource ambientSource;
    public AudioSource sfxSource;

    private const string PREF_MASTER_VOL = "MasterVolume_Pref";
    private const string PREF_AMBIENT_VOL = "AmbientVolume_Pref";
    private const string PREF_SFX_VOL = "SFXVolume_Pref";

    [SerializeField] AudioClip mainMenuBGM = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // If not found playerpref key, default set as 1
        //float savedMaster = PlayerPrefs.GetFloat(PREF_MASTER_VOL, 1f);
        //float savedAmbient = PlayerPrefs.GetFloat(PREF_AMBIENT_VOL, 1f);
        //float savedSFX = PlayerPrefs.GetFloat(PREF_SFX_VOL, 1f);

        //SetMasterVolume(savedMaster);
        //SetAmbientVolume(savedAmbient);
        //SetSFXVolume(savedSFX);

        if (mainMenuBGM != null)
        {
            PlayAmbientSound(mainMenuBGM);
        }
    }

    /// <summary>
    /// Play ambient music, if play the same clip, then ignore
    /// </summary>
    public void PlayAmbientSound(AudioClip clip)
    {
        if (clip == null) return;
        if (ambientSource.clip == clip && ambientSource.isPlaying) return;

        ambientSource.clip = clip;
        ambientSource.Play();
    }

    /// <summary>
    /// Stop ambient sound
    /// </summary>
    public void StopAmbientSound()
    {
        ambientSource.Stop();
    }

    /// <summary>
    /// Play sound effect
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// play sound effect at specific position, 3D sound
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position);
    }


    public void SetMasterVolume(float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("MasterVolume", db);

        PlayerPrefs.SetFloat(PREF_MASTER_VOL, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetAmbientVolume(float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("AmbientVolume", db);

        PlayerPrefs.SetFloat(PREF_AMBIENT_VOL, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        mainMixer.SetFloat("SFXVolume", db);

        PlayerPrefs.SetFloat(PREF_SFX_VOL, sliderValue);
        PlayerPrefs.Save();
    }

    public float GetSavedMasterVolume() => PlayerPrefs.GetFloat(PREF_MASTER_VOL, 1f);
    public float GetSavedAmbientVolume() => PlayerPrefs.GetFloat(PREF_AMBIENT_VOL, 1f);
    public float GetSavedSFXVolume() => PlayerPrefs.GetFloat(PREF_SFX_VOL, 1f);
}