using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 🐉
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 视距范围
    /// </summary>
    [SerializeField]
    Vector2 viewRange;

    [SerializeField]
    float scrollSpeed = 1f; // 滚动调整的速度

    InputSystem_Actions inputActions;

    /// <summary>
    /// 智能镜头控制器中控制镜头旋转的组件。
    /// </summary>
    CinemachineInputAxisController cinemaController;
    CinemachineOrbitalFollow orbitalFollow;

    private void Awake()
    {
        inputActions = new();
        cinemaController = FindFirstObjectByType<CinemachineInputAxisController>();
        orbitalFollow = cinemaController.transform.GetComponent<CinemachineOrbitalFollow>();
        cinemaController.enabled = false;
    }

    private void OnEnable()
    {
        // 通过监控触碰启用和禁用来实现是否镜头可旋转
        inputActions.Enable();
        inputActions.Player.Attack.started += ctx => cinemaController.enabled = true;
        inputActions.Player.Attack.canceled += ctx => cinemaController.enabled = false;
        // 通过滚轮来调整视距
        inputActions.UI.ScrollWheel.performed += ctx => orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + ctx.ReadValue<Vector2>().y * scrollSpeed, viewRange.x, viewRange.y);
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Attack.started -= ctx => cinemaController.enabled = true;
        inputActions.Player.Attack.canceled -= ctx => cinemaController.enabled = false;
        inputActions.UI.ScrollWheel.performed += ctx => orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + ctx.ReadValue<Vector2>().y * scrollSpeed, viewRange.x, viewRange.y);
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
