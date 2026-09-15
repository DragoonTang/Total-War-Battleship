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

    public event System.Action OnFired;

    private Tween _blinkTween;
    private bool _isReady;

    void Start()
    {
        fireButton.onClick.AddListener(OnFireButtonClicked);
        highlightOverlay.enabled = false;
    }

    private void OnFireButtonClicked()
    {
        bool fired = false;
        foreach (var w in linkedWeapons)
        {
            if (w != null && w.TryFire()) fired = true;
        }

        if (fired) OnFired?.Invoke();
    }

    void Update()
    {
        if (linkedWeapons == null || linkedWeapons.Length == 0) return;

        Weapon masterWeapon = linkedWeapons[0];
        cooldownMask.fillAmount = 1f - (masterWeapon.cooldownTimer / masterWeapon.cooldownTime);

        bool currentlyReady = masterWeapon.cooldownTimer <= 0;
        fireButton.interactable = currentlyReady;

        if (currentlyReady && !_isReady)
        {
            SetReadyState(true);
        }
        else if (!currentlyReady && _isReady)
        {
            SetReadyState(false);
        }

        _isReady = currentlyReady;
    }

    private void SetReadyState(bool ready)
    {
        fireButton.interactable = ready;

        if (ready)
        {
            highlightOverlay.enabled = true;
            _blinkTween = Tween.Alpha(highlightOverlay, 0f, 1f, 0.5f, cycles: -1, cycleMode: CycleMode.Yoyo);
        }
        else
        {
            if (_blinkTween.isAlive) _blinkTween.Stop();
            highlightOverlay.enabled = false;
        }
    }
}
