using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

[RequireComponent(typeof(Image))]
public class HealthBarImage : MonoBehaviour
{
    [Header("绑定设置")]
    [SerializeField] private Damageable targetDamageable;
    private Image fillImage;

    [Header("动画参数")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Tween healthTween;

    private void OnEnable()
    {
        fillImage = GetComponent<Image>();
        if (targetDamageable != null && fillImage != null)
        {
            // 注册事件
            targetDamageable.OnHit += HandleHitUpdate;
            targetDamageable.OnDie += HandleDeath;

            // 初始入场动画：从当前值平滑同步
            if (healthTween.isAlive) healthTween.Stop();
        }
    }

    private void OnDisable()
    {
        if (targetDamageable != null)
        {
            // 注销事件
            targetDamageable.OnHit -= HandleHitUpdate;
            targetDamageable.OnDie -= HandleDeath;
        }

        if (healthTween.isAlive) healthTween.Stop();
    }

    private void HandleHitUpdate(int _)
    {
        if (fillImage == null || targetDamageable == null) return;

        if (healthTween.isAlive) healthTween.Stop();

        // 更新血量条
        healthTween = Tween.UIFillAmount(fillImage, targetDamageable.Percent, duration, easeType);

        // UI 抖动反馈
        Tween.ShakeLocalPosition(GetComponent<RectTransform>(), strength: new Vector3(10f, 10f, 0), duration: 0.15f);
    }

    private void HandleDeath()
    {
        if (healthTween.isAlive) healthTween.Stop();

        // 死亡时 UI 缩放消失
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            Tween.Scale(rt, Vector3.zero, 0.2f, Ease.InBack)
                 .OnComplete(() => gameObject.SetActive(false));
        }
    }
}