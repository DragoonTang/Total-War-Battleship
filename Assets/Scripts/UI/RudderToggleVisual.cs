using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>舵向按钮保持按下的外观由 Toggle.isOn 驱动，与鼠标焦点无关。</summary>
[RequireComponent(typeof(UnityEngine.UI.Toggle))]
public class RudderToggleVisual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private UnityEngine.UI.Toggle toggle;
    [SerializeField] private RectTransform visualRoot;
    [SerializeField] private UnityEngine.UI.Image selectedImage;
    [SerializeField] private CanvasGroup visualGroup;
    [SerializeField] private bool breatheWhenSelected;
    [SerializeField] private float transitionDuration = 0.16f;
    [SerializeField] private float pressDepth = 2f;

    private Tween moveTween;
    private Tween fadeTween;
    private Tween breathTween;
    private bool pointerHeld;
    private bool wasInteractable;
    private Vector3 restPosition;

    private void Awake()
    {
        if (toggle == null) toggle = GetComponent<UnityEngine.UI.Toggle>();
        if (visualRoot != null) restPosition = visualRoot.localPosition;
    }

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(OnValueChanged);
        wasInteractable = toggle.IsInteractable();
        Refresh(true);
    }

    private void Update()
    {
        bool interactable = toggle.IsInteractable();
        if (interactable == wasInteractable) return;
        wasInteractable = interactable;
        if (!interactable) pointerHeld = false;
        Refresh(true);
    }

    private void OnValueChanged(bool _) => Refresh(false);

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !toggle.IsInteractable()) return;
        pointerHeld = true;
        Refresh(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        pointerHeld = false;
        Refresh(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerHeld = false;
        Refresh(false);
    }

    private void Refresh(bool instant)
    {
        StopTweens();
        if (visualRoot == null || selectedImage == null) return;

        bool pressed = toggle.isOn || pointerHeld;
        Vector3 position = restPosition + (pressed ? Vector3.down * pressDepth : Vector3.zero);
        if (visualGroup != null) visualGroup.alpha = toggle.IsInteractable() ? 1f : 0.45f;

        if (instant)
        {
            visualRoot.localPosition = position;
            SetSelectedAlpha(pressed ? 1f : 0f);
            StartBreathing();
            return;
        }

        moveTween = Tween.LocalPosition(visualRoot, position, transitionDuration, Ease.OutCubic, useUnscaledTime: true);
        fadeTween = Tween.Alpha(selectedImage, pressed ? 1f : 0f, transitionDuration, Ease.OutCubic, useUnscaledTime: true)
            .OnComplete(StartBreathing);
    }

    private void StartBreathing()
    {
        // 凹陷和金色标记始终保留；仅轻微改变选中图层亮度，回正保持常亮。
        if (!isActiveAndEnabled || !toggle.isOn || !breatheWhenSelected || !toggle.IsInteractable()) return;
        breathTween = Tween.Alpha(selectedImage, 0.94f, 1f, 0.8f, Ease.InOutSine,
            cycles: -1, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    private void SetSelectedAlpha(float alpha)
    {
        Color color = selectedImage.color;
        color.a = alpha;
        selectedImage.color = color;
    }

    private void OnDisable()
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(OnValueChanged);
        pointerHeld = false;
        StopTweens();
        if (visualRoot != null) visualRoot.localPosition = restPosition;
        if (selectedImage != null) SetSelectedAlpha(toggle != null && toggle.isOn ? 1f : 0f);
    }

    private void StopTweens()
    {
        if (moveTween.isAlive) moveTween.Stop();
        if (fadeTween.isAlive) fadeTween.Stop();
        if (breathTween.isAlive) breathTween.Stop();
    }
}
