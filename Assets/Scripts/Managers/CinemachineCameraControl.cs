using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CinemachineCameraControl : MonoBehaviour
{
    [SerializeField] Vector2 viewRange = new Vector2(2f, 20f);
    [SerializeField] float scrollSpeed = 0.1f;
    [SerializeField] CinemachineInputAxisController cinemaController;

    private CinemachineOrbitalFollow orbitalFollow;
    private InputSystem_Actions inputActions;
    private float lastDist;

    void Awake()
    {
        inputActions = new();
        orbitalFollow = cinemaController.GetComponent<CinemachineOrbitalFollow>();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.started += _ => cinemaController.enabled = true;
        inputActions.Player.Attack.canceled += _ => cinemaController.enabled = false;
        inputActions.UI.ScrollWheel.performed += ctx => ApplyZoom(-ctx.ReadValue<Vector2>().y * scrollSpeed);
    }

    void Update()
    {
        var touches = Touchscreen.current?.touches;
        if ((touches?.Count ?? 0) < 2) return;

        // 双指按下：禁用旋转防止冲突
        cinemaController.enabled = false;
        var p0 = touches.Value[0].position.ReadValue();
        var p1 = touches.Value[1].position.ReadValue();
        float dist = Vector2.Distance(p0, p1);

        if (touches.Value[1].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            ApplyZoom((lastDist - dist) * scrollSpeed * 0.1f);
        }
        lastDist = dist;
    }

    void ApplyZoom(float delta)
    {
        orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + delta, viewRange.x, viewRange.y);
    }

    void OnDisable() => inputActions.Disable();
}