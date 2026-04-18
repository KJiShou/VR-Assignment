using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HueSlider : MonoBehaviour
{
    [Header("UI 引用")]
    public Slider hueSlider;          // 拖入你魔改好的 Slider
    public Image svPaletteBaseImage;  // 拖入你上一讲做的 3层调色板的【最底层(Base_Color)】Image
    public SVPalette svPalette;
    public TextMeshProUGUI hexDisplayText;       // 拖入截图左下角的那个 HEX 显示文本

    void Start()
    {
        // 绑定滑动条的值改变事件
        hueSlider.onValueChanged.AddListener(UpdateBaseColor);

        // 初始化颜色
        UpdateBaseColor(hueSlider.value);
    }

    private void UpdateBaseColor(float currentHue)
    {
        // 【核心数学转换】：将 0~1 的 Hue 值转换为 RGB 纯色
        // ⚠️ 严厉注意：这里的 Saturation(饱和度) 和 Value(明度) 必须死死地锁在 1f！
        // 因为这是 Base Color，必须是最纯正、最亮的颜色。
        Color pureBaseColor = Color.HSVToRGB(currentHue, 1f, 1f);

        // 1. 将算出的纯色赋给 SV 调色板的底层
        if (svPaletteBaseImage != null)
        {
            svPaletteBaseImage.color = pureBaseColor;
        }

        if (svPalette != null) 
        {
            svPalette.material.SetColor("_BaseColor", pureBaseColor);
        }

        // 2. 将颜色转换为 HEX 字符串并显示
        if (hexDisplayText != null)
        {
            // ColorUtility 转换出的字符串不带 # 号，需要自己加上
            hexDisplayText.text = "#" + ColorUtility.ToHtmlStringRGB(pureBaseColor);
        }

        // 补充：如果你还需要把这个 Base Color 传给 SVPalette 脚本去计算最终颜色，
        // 可以在这里调用 SVPalette 的公开方法。
    }
}