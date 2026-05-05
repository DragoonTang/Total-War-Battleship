using PrimeTween;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TorpedoWeaponUI : MonoBehaviour
{
    [Header("绑定武器组")]
    // 在 Inspector 里把两个武器脚本都拖进去
    public Weapon[] linkedWeapons;

    [Header("UI 元素")]
    public Button fireButton;
    public Image cooldownMask;
    public Image highlightOverlay;

    private Tween _blinkTween;
    private bool _isReady;

    void Start()
    {
        fireButton.onClick.AddListener(OnFireButtonClicked);
        // 初始隐藏高亮（使用 PrimeTween 的静态方法设置初始状态更可靠）
        highlightOverlay.enabled=false;
    }

    private void OnFireButtonClicked()
    {
        // 只要点击，就尝试发射所有关联武器
        foreach (var w in linkedWeapons)
        {
            if (w != null) w.TryFire(); //
        }
    }

    void Update()
    {
        if (linkedWeapons == null || linkedWeapons.Length == 0) return;

        // 核心点：只检测第一个武器的冷却进度
        Weapon masterWeapon = linkedWeapons[0];

        cooldownMask.fillAmount = 1f - (masterWeapon.cooldownTimer / masterWeapon.cooldownTime);

        // 状态切换检测
        bool currentlyReady = masterWeapon.cooldownTimer <= 0; //[cite: 3]
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