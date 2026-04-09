using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

[System.Serializable]
public struct Step
{
    public AudioClip[] narratorSoundClips;
}

public class NarratorController : MonoBehaviour
{
    private AudioSource narratorSound;
    private bool clipIsEnd = false;

    [SerializeField] Step[] steps;

    [Header("Interrupt Sound (e.g. screaming/warning)")]
    private AudioSource interruptAudioSource;

    [Tooltip("Play after interrupt sound")]
    public AudioClip resumeClip;

    private bool isInterrupted = false;

    [Tooltip("Let player have time to prepare when load in this scene")]
    public float firstClipDelay = 1.5f;

    [Tooltip("Interval time for play sound to next sound")]
    public float playIntervalTime = 1.0f;

    [Tooltip("Delay time for after playing interrupt sound")]
    public float delayForResumeSound = 1.0f;

    [Tooltip("Delay time for after playing resume sound")]
    public float delayAfterResumeSound = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        narratorSound = GetComponent<AudioSource>();
        interruptAudioSource = gameObject.AddComponent<AudioSource>();
        interruptAudioSource.outputAudioMixerGroup = narratorSound.outputAudioMixerGroup;
        interruptAudioSource.spatialBlend = 1f;
        interruptAudioSource.rolloffMode = AudioRolloffMode.Linear;
        interruptAudioSource.playOnAwake = false;
    }

    public void PlayAudioClip(int step)
    {
        if (narratorSound != null && step >= 0 && step < steps.Length)
        {
            if (steps[step].narratorSoundClips != null && steps[step].narratorSoundClips.Length > 0)
            {
                StopAllCoroutines();
                narratorSound.Stop();
                interruptAudioSource.Stop();
                isInterrupted = false;
                clipIsEnd = false;
                StartCoroutine(PlayClipsSequentially(steps[step].narratorSoundClips, step));
            }
        }
    }

    private IEnumerator PlayClipsSequentially(AudioClip[] clips, int currentStep)
    {
        if (currentStep == 0)
        {
            yield return new WaitForSeconds(firstClipDelay);
        }

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                clipIsEnd = false;

                narratorSound.clip = clip;
                narratorSound.Play();

                // If sound still playing or interrupting, then stop here, wait until finish
                while (narratorSound.isPlaying || isInterrupted)
                {
                    yield return null;
                }

                clipIsEnd = true;
                
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // ==========================================
    // For event capture
    // ==========================================
    public void PlayInterruptSound(AudioClip specialClip)
    {
        if (isInterrupted) return;

        StartCoroutine(HandleInterruption(specialClip));
    }

    private IEnumerator HandleInterruption(AudioClip specialClip)
    {
        // Lock
        isInterrupted = true;

        if (narratorSound.isPlaying)
        {
            narratorSound.Pause();
        }

        interruptAudioSource.clip = specialClip;
        interruptAudioSource.Play();

        // Wait until special sound finish
        while (interruptAudioSource.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(delayForResumeSound);

        if (resumeClip != null && !clipIsEnd)
        {
            interruptAudioSource.clip = resumeClip;

            interruptAudioSource.Play();

            while (interruptAudioSource.isPlaying)
            {
                yield return null;
            }

            yield return new WaitForSeconds(delayAfterResumeSound);
        }

        // Back to main audio source progress
        narratorSound.UnPause();

        // Unlock
        isInterrupted = false;
    }
}
