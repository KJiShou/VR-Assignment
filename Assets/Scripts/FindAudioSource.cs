using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

public class FindAudioSource : MonoBehaviour
{
    private SimpleAudioFeedback simpleAudioFeedback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (AudioManager.Instance == null) return;
        if (AudioManager.Instance.sfxSource != null)
        {
            if (TryGetComponent<SimpleAudioFeedback>(out simpleAudioFeedback))
            {
                simpleAudioFeedback.audioSource = AudioManager.Instance.sfxSource;
            }
        }
    }
}
