using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class TorpedoWeaponUI : MonoBehaviour
{
    [Header("绑定武器组")]
    public Weapon[] linkedWeapons;

    [Header("UI 元素")]
    public Button fireButton;
    public Image cooldownMask;
    public Image highlightOverlay;

    [Header("HUD 状态")]
    [SerializeField] private Text stateText;
    [SerializeField] private Text cooldownText;
    [SerializeField] private RectTransform feedbackTarget;

    public event System.Action OnFired;

    private Tween blinkTween;
    private Tween fireFeedbackTween;
    private bool isReady;

    private void Start()
    {
        if (fireButton != null) fireButton.onClick.AddListener(OnFireButtonClicked);
        if (highlightOverlay != null) highlightOverlay.enabled = false;
        RefreshState(true);
    }

    private void OnDestroy()
    {
        if (fireButton != null) fireButton.onClick.RemoveListener(OnFireButtonClicked);
        StopTweens();
    }

    private void OnDisable()
    {
        StopTweens();
        if (feedbackTarget != null) feedbackTarget.localScale = Vector3.one;
    }

    private void OnFireButtonClicked()
    {
        bool fired = false;
        if (linkedWeapons != null)
        {
            foreach (Weapon weapon in linkedWeapons)
            {
                if (weapon != null && weapon.TryFire()) fired = true;
            }
        }

        if (!fired) return;

        PlayFireFeedback();
        OnFired?.Invoke();
    }

    private void Update()
    {
        RefreshState(false);
    }

    private void RefreshState(bool force)
    {
        Weapon masterWeapon = GetMasterWeapon();
        if (masterWeapon == null)
        {
            if (fireButton != null) fireButton.interactable = false;
            if (stateText != null) stateText.text = "NO TORPEDO";
            if (cooldownText != null) cooldownText.text = "--";
            if (cooldownMask != null) cooldownMask.fillAmount = 0f;
            return;
        }

        float cooldownTime = Mathf.Max(0.01f, masterWeapon.cooldownTime);
        float remaining = Mathf.Max(0f, masterWeapon.cooldownTimer);
        float progress = 1f - Mathf.Clamp01(remaining / cooldownTime);
        bool currentlyReady = remaining <= 0f;

        if (cooldownMask != null) cooldownMask.fillAmount = progress;
        if (fireButton != null) fireButton.interactable = currentlyReady;
        if (stateText != null) stateText.text = currentlyReady ? "READY" : "RELOADING";
        if (cooldownText != null) cooldownText.text = currentlyReady ? "" : remaining.ToString("0.0") + "s";

        if (force || currentlyReady != isReady) SetReadyState(currentlyReady);
        isReady = currentlyReady;
    }

    private Weapon GetMasterWeapon()
    {
        if (linkedWeapons == null) return null;
        foreach (Weapon weapon in linkedWeapons)
        {
            if (weapon != null) return weapon;
        }
        return null;
    }

    private void SetReadyState(bool ready)
    {
        if (blinkTween.isAlive) blinkTween.Stop();

        if (highlightOverlay == null) return;
        highlightOverlay.enabled = ready;

        if (ready)
        {
            blinkTween = Tween.Alpha(
                highlightOverlay,
                0.2f,
                0.72f,
                0.65f,
                Ease.InOutSine,
                cycles: -1,
                cycleMode: CycleMode.Yoyo,
                useUnscaledTime: true);
        }
    }

    private void PlayFireFeedback()
    {
        if (feedbackTarget == null) return;
        if (fireFeedbackTween.isAlive) fireFeedbackTween.Stop();

        fireFeedbackTween = Tween.Scale(
            feedbackTarget,
            Vector3.one * 0.9f,
            Vector3.one,
            0.24f,
            Ease.OutBack,
            useUnscaledTime: true);
    }

    private void StopTweens()
    {
        if (blinkTween.isAlive) blinkTween.Stop();
        if (fireFeedbackTween.isAlive) fireFeedbackTween.Stop();
    }
}
