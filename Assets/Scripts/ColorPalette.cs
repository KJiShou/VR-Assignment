using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPalette : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("UI References")]
    [Tooltip("Color palette itself RectTransform")]
    public RectTransform paletteRect;
    [Tooltip("The circle follow by ray (UI Image)")]
    public RectTransform pickerCursor;
    public Slider hueSlider; 
    public Image svPaletteBaseImage;
    public TextMeshProUGUI hexDisplayText;
    public Image displayImage;

    public PaintDrawing paintDrawing;

    [Header("Output Data")]
    public float currentSaturation = 0f;
    public float currentValue = 1f;

    void Start()
    {
        hueSlider.onValueChanged.AddListener(OnHueSliderChanged);

        UpdateSVFromLocalPosition(pickerCursor.localPosition);

        UpdateBaseColorUI(hueSlider.value);

        UpdateFinalMaterialAndText();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ProcessRaycastInput(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ProcessRaycastInput(eventData);
    }

    private void ProcessRaycastInput(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(paletteRect, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        // Limit UI circle movement in frame
        localCursor.x = Mathf.Clamp(localCursor.x, paletteRect.rect.xMin, paletteRect.rect.xMax);
        localCursor.y = Mathf.Clamp(localCursor.y, paletteRect.rect.yMin, paletteRect.rect.yMax);
        pickerCursor.localPosition = localCursor;

        UpdateSVFromLocalPosition(localCursor);

        UpdateFinalMaterialAndText();
    }

    // Based on local pos to calculate Saturation and Value (Transparency)
    private void UpdateSVFromLocalPosition(Vector2 localPos)
    {
        // Left is white, Right is base color ( - 1f)
        currentSaturation = 1f - Mathf.InverseLerp(paletteRect.rect.xMin, paletteRect.rect.xMax, localPos.x);
        // Top is base color, Bottom is black
        currentValue = Mathf.InverseLerp(paletteRect.rect.yMin, paletteRect.rect.yMax, localPos.y);
    }

    private void OnHueSliderChanged(float newHue)
    {
        UpdateBaseColorUI(newHue);
        UpdateFinalMaterialAndText();
    }

    private void UpdateBaseColorUI(float currentHue)
    {
        if (svPaletteBaseImage != null)
        {
            svPaletteBaseImage.color = Color.HSVToRGB(currentHue, 1f, 1f);
        }
    }

    private void UpdateFinalMaterialAndText()
    {
        // Final color = current hue + calculated saturation + calculated lightness
        Color finalColor = Color.HSVToRGB(hueSlider.value, currentSaturation, currentValue);

        if (paintDrawing != null)
        {
            paintDrawing.color = finalColor;
        }

        if (hexDisplayText != null)
        {
            hexDisplayText.text = "#" + ColorUtility.ToHtmlStringRGB(finalColor);
        }

        if(displayImage != null)
        {
            displayImage.color = finalColor;
        }
    }
}
