using System.Collections.Generic;
using UnityEngine;

public class PaintDrawing : MonoBehaviour
{
    public GameObject linePrefab; // 预制体必须带有 LineRenderer，且 Use World Space 设为 False
    public Transform penTip;      // 笔尖
    public LayerMask canvasLayer;

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();
    private Transform currentCanvas;

    private float width = 0.02f;
    private Color color = Color.white;

    public void StartTrail()
    {
        RaycastHit hit;
        // 1. 射线检测确定画布
        if (Physics.Raycast(penTip.position, penTip.forward, out hit, 0.5f, canvasLayer))
        {
            Debug.Log("<color=green>【成功】射线击中画布！开始生成笔迹。</color>");
            currentCanvas = hit.transform;

            // 2. 生成线条并将其设为画布的子物体
            GameObject lineObj = Instantiate(linePrefab, hit.point, Quaternion.identity, currentCanvas);
            currentLine = lineObj.GetComponent<LineRenderer>();

            if (currentLine == null)
            {
                Debug.LogError("【致命错误】你的 linePrefab 上没有挂载 LineRenderer 组件！");
                return;
            }

            // [极其关键] 强制关闭世界坐标，使用本地坐标绘制
            currentLine.useWorldSpace = false;

            points.Clear();
            AddPoint();
        }
        else
        {
            Debug.LogWarning("【失败】射线没有打中画布！请检查：1.画布有Collider吗？2.LayerMask对吗？3.笔尖蓝色箭头朝向对吗？");
        }
    }

    public void Update()
    {
        if (currentLine != null)
        {
            AddPoint();
        }
    }

    private void AddPoint()
    {
        RaycastHit hit;
        // 核心升级：Update 里也使用射线检测。
        // 这样无论你的手腕怎么抖，或者笔尖戳进了画布多深，
        // 我们永远只取射线在“画布表面”的碰撞点！
        if (Physics.Raycast(penTip.position, penTip.forward, out hit, 0.5f, canvasLayer))
        {
            // 防穿模终极武器：将点沿着画布法线（往外）偏移 1 毫米，确保线永远浮在表面！
            Vector3 offsetPoint = hit.point + (hit.normal * 0.001f);

            // 转换为本地坐标
            Vector3 localPos = currentCanvas.InverseTransformPoint(offsetPoint);

            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], localPos) > 0.005f)
            {
                points.Add(localPos);
                currentLine.positionCount = points.Count;
                currentLine.SetPosition(points.Count - 1, localPos);
            }
        }
    }

    public void EndTrail()
    {
        currentLine = null;
    }
}
