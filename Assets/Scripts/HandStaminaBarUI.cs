using UnityEngine;
using UnityEngine.UI;

public class HandStaminaBarUI : MonoBehaviour
{
    [Header("References")]
    public StaminaManager staminaManager;
    public bool isLeftHand = true;

    [Header("Bar References")]
    public RectTransform barRoot;
    public RectTransform blueBar;
    public RectTransform whiteBar;
    public RectTransform redBar;

    [Header("Images")]
    public RawImage whiteImage;

    [Header("Optional")]
    public Canvas staminaCanvas;
    public bool showOnlyWhenHolding = true;

    void Start()
    {
        if (staminaManager == null)
            staminaManager = FindFirstObjectByType<StaminaManager>();
    }

    void Update()
    {
        if (staminaManager == null || barRoot == null || blueBar == null || whiteBar == null || redBar == null)
            return;

        float currentStamina;
        float baseMax;
        float chalkBonus;
        bool chalkActive;
        float chalkTimeRemaining;
        float chalkDuration;
        bool isHolding;

        if (isLeftHand)
        {
            currentStamina = staminaManager.currentLeftStamina;
            baseMax = staminaManager.baseMaxLeftStamina;
            chalkBonus = staminaManager.chalkBonus;
            chalkActive = staminaManager.leftChalkActive;
            chalkTimeRemaining = staminaManager.GetLeftChalkTimeRemaining();
            chalkDuration = staminaManager.chalkDuration;
            isHolding = staminaManager.isLeftHolding;
        }
        else
        {
            currentStamina = staminaManager.currentRightStamina;
            baseMax = staminaManager.baseMaxRightStamina;
            chalkBonus = staminaManager.chalkBonus;
            chalkActive = staminaManager.rightChalkActive;
            chalkTimeRemaining = staminaManager.GetRightChalkTimeRemaining();
            chalkDuration = staminaManager.chalkDuration;
            isHolding = staminaManager.isRightHolding;
        }

        if (staminaCanvas != null && showOnlyWhenHolding)
            staminaCanvas.enabled = isHolding;

        float currentMax = chalkActive ? baseMax + chalkBonus : baseMax;
        float unitWidth = barRoot.rect.width / baseMax;

        float blueWidth = baseMax * unitWidth;
        float whiteWidth = chalkActive ? chalkBonus * unitWidth : 0f;

        float missingStamina = currentMax - currentStamina;
        float redWidth = missingStamina * unitWidth;

        blueBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, blueWidth);
        whiteBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, whiteWidth);
        redBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, redWidth);

        blueBar.anchoredPosition = new Vector2(0f, blueBar.anchoredPosition.y);
        whiteBar.anchoredPosition = new Vector2(blueWidth, whiteBar.anchoredPosition.y);

        float totalBarWidth = blueWidth + whiteWidth;
        redBar.anchoredPosition = new Vector2(totalBarWidth - redWidth, redBar.anchoredPosition.y);

        if (whiteImage != null)
        {
            if (chalkActive)
            {
                whiteBar.gameObject.SetActive(true);

                float chalkPercent = chalkDuration > 0f ? Mathf.Clamp01(chalkTimeRemaining / (chalkDuration * 2)) : 0f;

                Color c = whiteImage.color;

                float baseAlpha = chalkPercent * 0.8f;

                if (chalkPercent > 0f && chalkPercent <= 0.1f)
                {
                    float blink = Mathf.PingPong(Time.time * 5f, 0.7f);
                    c.a = blink;
                }
                else
                {
                    c.a = baseAlpha;
                }

                whiteImage.color = c;
            }
            else
            {
                whiteBar.gameObject.SetActive(false);
            }
        }
    }
}