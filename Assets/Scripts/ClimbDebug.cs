using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ClimbDebug : MonoBehaviour
{
    private XRBaseInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();

        // 监听抓取和松开事件
        _interactable.selectEntered.AddListener(OnGrab);
        _interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // 绿色代表成功抓取
        Debug.Log($"<color=green>[CLIMB]</color> 成功！控制器 <b>{args.interactorObject.transform.name}</b> 抓住了 <b>{gameObject.name}</b>");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // 红色代表松开
        Debug.Log($"<color=red>[CLIMB]</color> 松开。控制器 <b>{args.interactorObject.transform.name}</b> 离开了 <b>{gameObject.name}</b>");
    }

    void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnGrab);
            _interactable.selectExited.RemoveListener(OnRelease);
        }
    }
}
