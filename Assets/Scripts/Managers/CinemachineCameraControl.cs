using UnityEngine;
using UnityEngine.InputSystem;
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
    private System.Action<InputAction.CallbackContext> onScrollWheel;

    // 双指捏合状态
    private float lastPinchDist;
    private bool wasPinching;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        orbitalFollow = cinemaController.GetComponent<CinemachineOrbitalFollow>();

        onAttackStarted  = _ => cinemaController.enabled = true;
        onAttackCanceled = _ => cinemaController.enabled = false;
        onScrollWheel    = ctx => ApplyZoom(-ctx.ReadValue<Vector2>().y * scrollSpeed);
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.started   += onAttackStarted;
        inputActions.Player.Attack.canceled  += onAttackCanceled;
        inputActions.UI.ScrollWheel.performed += onScrollWheel;
        cinemaController.enabled = false;
    }

    void OnDisable()
    {
        inputActions.Player.Attack.started   -= onAttackStarted;
        inputActions.Player.Attack.canceled  -= onAttackCanceled;
        inputActions.UI.ScrollWheel.performed -= onScrollWheel;
        inputActions.Disable();
    }

    void Update()
    {
        HandlePinchZoom();
    }

    /// <summary>
    /// 双指捏合缩放（安卓）。
    /// 捏合期间禁用镜头旋转，捏合结束后若仍有单指按下则恢复旋转。
    /// </summary>
    private void HandlePinchZoom()
    {
        int activeCount = GetActiveTouchCount();

        if (activeCount >= 2)
        {
            // 进入双指模式：禁止镜头旋转
            cinemaController.enabled = false;

            var t0 = Touchscreen.current.touches[0];
            var t1 = Touchscreen.current.touches[1];
            float dist = Vector2.Distance(
                t0.position.ReadValue(),
                t1.position.ReadValue());

            // 任意一指移动时计算缩放量
            var phase0 = t0.phase.ReadValue();
            var phase1 = t1.phase.ReadValue();
            if (phase0 == TouchPhase.Moved || phase1 == TouchPhase.Moved)
            {
                ApplyZoom((lastPinchDist - dist) * scrollSpeed * 0.1f);
            }

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

    /// <summary>
    /// 统计当前活跃触点数（排除 None / Ended / Canceled 状态的槽位）。
    /// Touchscreen.touches.Count 是设备最大槽位数，不代表实际触点数。
    /// </summary>
    private int GetActiveTouchCount()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return 0;

        int count = 0;
        foreach (var touch in touchscreen.touches)
        {
            var phase = touch.phase.ReadValue();
            if (phase != TouchPhase.None &&
                phase != TouchPhase.Ended &&
                phase != TouchPhase.Canceled)
            {
                count++;
            }
        }
        return count;
    }

    private void ApplyZoom(float delta)
    {
        orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + delta, viewRange.x, viewRange.y);
    }
}
