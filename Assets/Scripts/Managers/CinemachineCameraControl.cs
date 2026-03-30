using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; // TouchControl
using Unity.Cinemachine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase; // 消除与旧版 UnityEngine.TouchPhase 的歧义

/// <summary>
/// 镜头控制：统一管理 PC（鼠标左键旋转 + 滚轮缩放）与安卓（单指拖拽旋转 + 双指捏合缩放）。
/// </summary>
public class CinemachineCameraControl : MonoBehaviour
{
    [SerializeField] Vector2 viewRange = new Vector2(2f, 20f);
    [SerializeField] float scrollSpeed = 0.1f;
    [SerializeField] CinemachineInputAxisController cinemaController;

    private CinemachineOrbitalFollow orbitalFollow;
    private InputSystem_Actions inputActions;

    // 缓存 lambda，确保 -= 能正确取消订阅
    private System.Action<InputAction.CallbackContext> onAttackStarted;
    private System.Action<InputAction.CallbackContext> onAttackCanceled;
    private System.Action<InputAction.CallbackContext> onScrollZoom;

    /// <summary>
    /// 独立 InputAction，直接绑定 &lt;Mouse&gt;/scroll/y，不属于任何 ActionMap。
    /// 完全绕开 EventSystem / InputSystemUIInputModule 的事件消费，
    /// 同时以回调方式捕获 delta 型滚轮事件，避免 Update 轮询时值已被帧边界清零的问题。
    /// </summary>
    private InputAction scrollZoomAction;

    // 双指捏合状态
    private float lastPinchDist;
    private bool wasPinching;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        orbitalFollow = cinemaController.GetComponent<CinemachineOrbitalFollow>();

        onAttackStarted = _ => cinemaController.enabled = true;
        onAttackCanceled = _ => cinemaController.enabled = false;

        // 独立 Action：不绑定 ActionMap，EventSystem 不可见也不会消费它
        scrollZoomAction = new InputAction(
            name: "ScrollZoom",
            type: InputActionType.Value,
            binding: "<Mouse>/scroll/y");
        onScrollZoom = ctx => ApplyZoom(-ctx.ReadValue<float>() * scrollSpeed);
        scrollZoomAction.performed += onScrollZoom;
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.started  += onAttackStarted;
        inputActions.Player.Attack.canceled += onAttackCanceled;
        scrollZoomAction.Enable();
        cinemaController.enabled = false;
    }

    void OnDisable()
    {
        inputActions.Player.Attack.started  -= onAttackStarted;
        inputActions.Player.Attack.canceled -= onAttackCanceled;
        scrollZoomAction.Disable();
        inputActions.Disable();
    }

    void OnDestroy()
    {
        scrollZoomAction.performed -= onScrollZoom;
        scrollZoomAction.Dispose();
    }

    void Update()
    {
        HandlePinchZoom();
    }

    /// <summary>
    /// 双指捏合缩放（安卓 / 编辑器 Device Simulator）。
    /// 捏合期间禁用镜头旋转，捏合结束后若仍有单指按下则恢复旋转。
    /// 遍历所有触点槽位收集活跃触点，避免固定下标带来的槽位错位问题。
    /// </summary>
    private void HandlePinchZoom()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        // 收集活跃触点（跳过 None / Ended / Canceled 槽位）
        TouchControl touch0 = null, touch1 = null;
        int activeCount = 0;
        foreach (var touch in touchscreen.touches)
        {
            var phase = touch.phase.ReadValue();
            if (phase == TouchPhase.None || phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
                continue;

            if (activeCount == 0)      touch0 = touch;
            else if (activeCount == 1) touch1 = touch;

            activeCount++;
            if (activeCount >= 2) break;
        }

        if (activeCount >= 2)
        {
            // 进入双指模式：禁止镜头旋转
            cinemaController.enabled = false;

            float dist = Vector2.Distance(
                touch0.position.ReadValue(),
                touch1.position.ReadValue());

            // 跳过第一帧（lastPinchDist 尚未初始化），从第二帧起计算缩放量
            if (wasPinching)
                ApplyZoom((lastPinchDist - dist) * scrollSpeed * 0.1f);

            lastPinchDist = dist;
            wasPinching = true;
        }
        else if (wasPinching)
        {
            // 从双指过渡到单指或全部抬起
            wasPinching = false;
            // 单指仍在屏幕上时，恢复镜头旋转（Attack.started 已经触发过了）
            cinemaController.enabled = (activeCount == 1);
        }
    }

    private void ApplyZoom(float delta)
    {
        orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + delta, viewRange.x, viewRange.y);
    }
}
