using UnityEngine;

public class DamageOverlayUI : MonoBehaviour
{
    public static DamageOverlayUI Instance { get; private set; }

    [Header("References")]
    public Canvas overlayCanvas;
    public UnityEngine.UI.Image overlayImage;

    [Header("Timing")]
    public float fadeInDuration = 0.15f;
    public float holdDuration = 0.2f;
    public float fadeOutDuration = 0.15f;

    [Header("Color")]
    public Color overlayColor = new Color(1f, 0f, 0f, 0.6f);

    private float _timer = 0f;
    private bool _isActive = false;
    private enum FadeState { FadeIn, Hold, FadeOut }
    private FadeState _state;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (overlayCanvas == null)
        {
            overlayCanvas = GetComponent<Canvas>();
        }

        if (overlayImage == null)
        {
            Transform child = transform.Find("OverlayImage");
            if (child != null)
            {
                overlayImage = child.GetComponent<UnityEngine.UI.Image>();
            }
        }

        if (overlayImage != null)
        {
            Color c = overlayColor;
            c.a = 0f;
            overlayImage.color = c;
        }
    }

    void Update()
    {
        if (!_isActive) return;

        _timer += Time.deltaTime;

        switch (_state)
        {
            case FadeState.FadeIn:
                if (_timer >= fadeInDuration)
                {
                    SetAlpha(overlayColor.a);
                    _state = FadeState.Hold;
                    _timer = 0f;
                }
                else
                {
                    float t = _timer / fadeInDuration;
                    SetAlpha(Mathf.Lerp(0f, overlayColor.a, t));
                }
                break;

            case FadeState.Hold:
                if (_timer >= holdDuration)
                {
                    _state = FadeState.FadeOut;
                    _timer = 0f;
                }
                break;

            case FadeState.FadeOut:
                if (_timer >= fadeOutDuration)
                {
                    SetAlpha(0f);
                    _isActive = false;
                }
                else
                {
                    float t = _timer / fadeOutDuration;
                    SetAlpha(Mathf.Lerp(overlayColor.a, 0f, t));
                }
                break;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = alpha;
            overlayImage.color = c;
        }
    }

    public void TriggerDamageFlash()
    {
        if (!_isActive)
        {
            _isActive = true;
            _state = FadeState.FadeIn;
            _timer = 0f;
        }
        else
        {
            _state = FadeState.FadeIn;
            _timer = 0f;
        }
    }
}
