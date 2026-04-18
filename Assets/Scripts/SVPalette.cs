using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 必须实现拖拽和点击接口
public class SVPalette : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("UI 引用")]
    public RectTransform paletteRect; // 调色板自身的 RectTransform
    public RectTransform pickerCursor; // 那个跟着射线移动的圆圈 (UI Image)
    public Material material;

    [Header("输出数据")]
    public float currentSaturation = 0f;
    public float currentValue = 1f;

    // 当 VR 射线按下时触发
    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    // 当 VR 射线按住并拖拽时触发
    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    private void UpdateColor(PointerEventData eventData)
    {
        Vector2 localCursor;
        // 核心纠错：不要自己算世界坐标到本地坐标的转换！
        // 用 RectTransformUtility 把射线的屏幕/世界点击位置，精准转换为 UI 内部的本地 2D 坐标
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(paletteRect, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        // 获取面板的宽高
        float width = paletteRect.rect.width;
        float height = paletteRect.rect.height;

        // 将本地坐标限制在面板范围内 (防止圆圈跑到外面)
        localCursor.x = Mathf.Clamp(localCursor.x, paletteRect.rect.xMin, paletteRect.rect.xMax);
        localCursor.y = Mathf.Clamp(localCursor.y, paletteRect.rect.yMin, paletteRect.rect.yMax);

        // 移动圆圈 UI 的位置
        pickerCursor.localPosition = localCursor;

        // 计算归一化数值 (0 到 1)
        // X轴代表饱和度 (Saturation)
        currentSaturation = 1f - Mathf.InverseLerp(paletteRect.rect.xMin, paletteRect.rect.xMax, localCursor.x);
        // Y轴代表明度 (Value)
        currentValue = Mathf.InverseLerp(paletteRect.rect.yMin, paletteRect.rect.yMax, localCursor.y);

        Debug.Log($"Saturation: {currentSaturation}, Transparency: {currentValue}");
        // 通知颜色管理器更新最终颜色
        // ColorManager.Instance.UpdateFinalColor(); 
        Color finalColor = Color.HSVToRGB(0, currentSaturation, currentValue);

        // 将颜色应用到模型 (假设是改变材质的基础色)
        material.SetColor("_BaseColor", finalColor);
    }
}