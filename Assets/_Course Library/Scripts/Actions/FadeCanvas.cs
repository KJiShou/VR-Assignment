using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Fades a canvas over time using a coroutine and a canvas group
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [Tooltip("The speed at which the canvas fades")]
    public float defaultDuration = 1.0f;

    public Coroutine CurrentRoutine { private set; get; } = null;

    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    private CanvasGroup canvasGroup = null;
    private Canvas canvas = null;

    private float alpha = 0.0f;

    private float quickFadeDuration = 0.25f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        StartFadeOut();
    }

    public void StartFadeIn()
    {
        StopAllCoroutines();
        // CurrentRoutine = StartCoroutine(FadeIn(defaultDuration));
        CurrentRoutine = StartCoroutine(FadeRoutine(canvasGroup.alpha, 1.0f, defaultDuration));
    }

    public void StartFadeOut()
    {
        StopAllCoroutines();
        // CurrentRoutine = StartCoroutine(FadeOut(defaultDuration));
        CurrentRoutine = StartCoroutine(FadeRoutine(canvasGroup.alpha, 0.0f, defaultDuration));
    }

    public void QuickFadeIn()
    {
        StopAllCoroutines();
        // CurrentRoutine = StartCoroutine(FadeIn(quickFadeDuration));
        CurrentRoutine = StartCoroutine(FadeRoutine(canvasGroup.alpha, 1.0f, quickFadeDuration));
    }

    public void QuickFadeOut()
    {
        StopAllCoroutines();
        // CurrentRoutine = StartCoroutine(FadeOut(quickFadeDuration));
        CurrentRoutine = StartCoroutine(FadeRoutine(canvasGroup.alpha, 0.0f, quickFadeDuration));
    }

    public void FadeInAndOutWithDelay(float waitTimeInBlack)
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeSequence(waitTimeInBlack));
    }

    private IEnumerator FadeSequence(float waitTime)
    {
        // Wait until become black screen
        yield return StartCoroutine(FadeRoutine(canvasGroup.alpha, 1.0f, defaultDuration));

        // Stay back screen for few seconds
        yield return new WaitForSeconds(waitTime);

        // Start to become transparent
        yield return StartCoroutine(FadeRoutine(1.0f, 0.0f, defaultDuration));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration)
    {
        Debug.Log("Start FadeRoutine");
        // if back screen, open block RayCast, prevent player interact with the scene

        if (targetAlpha > 0f)
        {
            canvas.enabled = true;
            canvasGroup.blocksRaycasts = true;
            if (leftRayInteractor != null) leftRayInteractor.enabled = false;
            if (rightRayInteractor != null) rightRayInteractor.enabled = false;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (targetAlpha == 0f)
        {
            canvas.enabled = false;
            canvasGroup.blocksRaycasts = false;
            if (leftRayInteractor != null) leftRayInteractor.enabled = true;
            if (rightRayInteractor != null) rightRayInteractor.enabled = true;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        gameObject.SetActive(true);
        float elapsedTime = 0.0f;

        while (alpha <= 1.0f)
        {
            SetAlpha(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0.0f;

        while (alpha >= 0.0f)
        {
            SetAlpha(1 - (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void SetAlpha(float value)
    {
        alpha = value;
        canvasGroup.alpha = alpha;
    }
}