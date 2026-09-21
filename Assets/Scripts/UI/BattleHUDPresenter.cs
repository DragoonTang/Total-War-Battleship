using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleHUDPresenter : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private Toggle[] speedToggles;
    [SerializeField] private Toggle[] rudderToggles;
    [SerializeField] private RectTransform speedSelectionFrame;
    [SerializeField] private RectTransform rudderSelectionFrame;
    [SerializeField] private CanvasGroup rudderSelectionCanvasGroup;
    [SerializeField] private Text speedStateText;
    [SerializeField] private Text rudderStateText;

    [Header("Health")]
    [SerializeField] private HealthBarImage healthBar;
    [SerializeField] private Damageable healthSource;
    [SerializeField] private Text healthValueText;

    [Header("Animation")]
    [SerializeField] private float selectorPadding = 8f;
    [SerializeField] private float selectorMoveDuration = 0.2f;

    private UnityAction<bool>[] speedListeners;
    private UnityAction<bool>[] rudderListeners;
    private Coroutine initializeRoutine;
    private Tween speedMoveTween;
    private Tween speedSizeTween;
    private Tween rudderMoveTween;
    private Tween rudderSizeTween;
    private Tween rudderBreathTween;
    private int lastHealth = int.MinValue;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private bool subscribed;

    private void OnEnable()
    {
        Subscribe();
        initializeRoutine = StartCoroutine(InitializeAfterLayout());
    }

    private void OnDisable()
    {
        Unsubscribe();
        if (initializeRoutine != null) StopCoroutine(initializeRoutine);
        initializeRoutine = null;
        StopAllTweens();
    }

    private IEnumerator InitializeAfterLayout()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        RefreshAll(true);
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        initializeRoutine = null;
    }

    private void Update()
    {
        UpdateHealthText();

        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            Canvas.ForceUpdateCanvases();
            RefreshAll(true);
        }
    }

    private void Subscribe()
    {
        if (subscribed) return;
        subscribed = true;

        speedListeners = CreateListeners(speedToggles, OnSpeedChanged);
        rudderListeners = CreateListeners(rudderToggles, OnRudderChanged);
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        subscribed = false;

        RemoveListeners(speedToggles, speedListeners);
        RemoveListeners(rudderToggles, rudderListeners);
        speedListeners = null;
        rudderListeners = null;
    }

    private static UnityAction<bool>[] CreateListeners(Toggle[] toggles, System.Action<int, bool> callback)
    {
        if (toggles == null) return null;

        var listeners = new UnityAction<bool>[toggles.Length];
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            listeners[i] = isOn => callback(index, isOn);
            if (toggles[i] != null) toggles[i].onValueChanged.AddListener(listeners[i]);
        }

        return listeners;
    }

    private static void RemoveListeners(Toggle[] toggles, UnityAction<bool>[] listeners)
    {
        if (toggles == null || listeners == null) return;

        int count = Mathf.Min(toggles.Length, listeners.Length);
        for (int i = 0; i < count; i++)
        {
            if (toggles[i] != null && listeners[i] != null)
                toggles[i].onValueChanged.RemoveListener(listeners[i]);
        }
    }

    private void OnSpeedChanged(int index, bool isOn)
    {
        if (!isOn || !IsValid(speedToggles, index)) return;
        SetSelector(speedSelectionFrame, speedToggles[index].transform as RectTransform, false, ref speedMoveTween, ref speedSizeTween);
        if (speedStateText != null) speedStateText.text = SpeedLabel(speedToggles[index].name);
    }

    private void OnRudderChanged(int index, bool isOn)
    {
        if (!isOn || !IsValid(rudderToggles, index)) return;
        SetSelector(rudderSelectionFrame, rudderToggles[index].transform as RectTransform, false, ref rudderMoveTween, ref rudderSizeTween);
        if (rudderStateText != null) rudderStateText.text = RudderLabel(rudderToggles[index].name);
        SetRudderBreathing(rudderToggles[index].name != "0");
    }

    private void RefreshAll(bool instant)
    {
        int speedIndex = FindActive(speedToggles);
        if (speedIndex >= 0)
        {
            SetSelector(speedSelectionFrame, speedToggles[speedIndex].transform as RectTransform, instant, ref speedMoveTween, ref speedSizeTween);
            if (speedStateText != null) speedStateText.text = SpeedLabel(speedToggles[speedIndex].name);
        }

        int rudderIndex = FindActive(rudderToggles);
        if (rudderIndex >= 0)
        {
            SetSelector(rudderSelectionFrame, rudderToggles[rudderIndex].transform as RectTransform, instant, ref rudderMoveTween, ref rudderSizeTween);
            if (rudderStateText != null) rudderStateText.text = RudderLabel(rudderToggles[rudderIndex].name);
            SetRudderBreathing(rudderToggles[rudderIndex].name != "0");
        }

        UpdateHealthText(true);
    }

    private void SetSelector(RectTransform selector, RectTransform target, bool instant, ref Tween moveTween, ref Tween sizeTween)
    {
        if (selector == null || target == null || selector.parent is not RectTransform selectorParent) return;

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(selectorParent, target);
        Vector3 desiredPosition = new(bounds.center.x, bounds.center.y, selector.localPosition.z);
        Vector2 desiredSize = new(bounds.size.x + selectorPadding * 2f, bounds.size.y + selectorPadding * 2f);

        if (moveTween.isAlive) moveTween.Stop();
        if (sizeTween.isAlive) sizeTween.Stop();

        if (instant)
        {
            selector.localPosition = desiredPosition;
            selector.sizeDelta = desiredSize;
            return;
        }

        moveTween = Tween.LocalPosition(selector, desiredPosition, selectorMoveDuration, Ease.OutCubic, useUnscaledTime: true);
        sizeTween = Tween.UISizeDelta(selector, desiredSize, selectorMoveDuration, Ease.OutCubic, useUnscaledTime: true);
    }

    private void SetRudderBreathing(bool enabled)
    {
        if (rudderBreathTween.isAlive) rudderBreathTween.Stop();
        if (rudderSelectionFrame != null) rudderSelectionFrame.localScale = Vector3.one;

        if (rudderSelectionCanvasGroup != null)
            rudderSelectionCanvasGroup.alpha = enabled ? 0.72f : 1f;

        if (!enabled || rudderSelectionCanvasGroup == null) return;

        rudderBreathTween = Tween.Alpha(
            rudderSelectionCanvasGroup,
            0.72f,
            1f,
            0.8f,
            Ease.InOutSine,
            cycles: -1,
            cycleMode: CycleMode.Yoyo,
            useUnscaledTime: true);
    }

    private void UpdateHealthText(bool force = false)
    {
        Damageable source = healthSource != null ? healthSource : healthBar == null ? null : healthBar.TargetDamageable;
        if (source == null || healthValueText == null) return;

        int maxHealth = Mathf.Max(1, source.maxHP);
        int currentHealth = Mathf.Clamp(Mathf.CeilToInt(source.Percent * maxHealth), 0, maxHealth);
        if (!force && currentHealth == lastHealth) return;

        lastHealth = currentHealth;
        healthValueText.text = currentHealth + " / " + maxHealth;
    }

    private void StopAllTweens()
    {
        if (speedMoveTween.isAlive) speedMoveTween.Stop();
        if (speedSizeTween.isAlive) speedSizeTween.Stop();
        if (rudderMoveTween.isAlive) rudderMoveTween.Stop();
        if (rudderSizeTween.isAlive) rudderSizeTween.Stop();
        if (rudderBreathTween.isAlive) rudderBreathTween.Stop();

        if (speedSelectionFrame != null) speedSelectionFrame.localScale = Vector3.one;
        if (rudderSelectionFrame != null) rudderSelectionFrame.localScale = Vector3.one;
        if (rudderSelectionCanvasGroup != null) rudderSelectionCanvasGroup.alpha = 1f;
    }

    private static int FindActive(Toggle[] toggles)
    {
        if (toggles == null) return -1;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i] != null && toggles[i].isOn) return i;
        }
        return -1;
    }

    private static bool IsValid(Toggle[] toggles, int index)
    {
        return toggles != null && index >= 0 && index < toggles.Length && toggles[index] != null;
    }

    private static string SpeedLabel(string toggleName)
    {
        return toggleName switch
        {
            "1" => "AHEAD 4",
            "0.75" => "AHEAD 3",
            "0.5" => "AHEAD 2",
            "0.25" => "AHEAD 1",
            "0" => "STOP",
            "-0.5" => "REVERSE",
            _ => toggleName
        };
    }

    private static string RudderLabel(string toggleName)
    {
        return toggleName switch
        {
            "-1" => "RUDDER LEFT",
            "0" => "RUDDER MID",
            "1" => "RUDDER RIGHT",
            _ => toggleName
        };
    }
}
