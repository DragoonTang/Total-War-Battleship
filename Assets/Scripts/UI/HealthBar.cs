using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

[RequireComponent(typeof(Image))]
public class HealthBarImage : MonoBehaviour
{
    [Header("绑定设置")]
    [SerializeField] private Damageable targetDamageable;
    private Image fillImage;
    public Damageable TargetDamageable => targetDamageable;

    [Header("动画参数")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Tween healthTween;
    private Tween shakeTween;
    private Tween deathTween;
    private Vector3 baseLocalPosition;
    private Vector3 baseLocalScale;

    private void OnEnable()
    {
        fillImage = GetComponent<Image>();
        baseLocalPosition = transform.localPosition;
        baseLocalScale = transform.localScale;
        if (targetDamageable == null || fillImage == null) return;

        targetDamageable.OnHit += HandleHitUpdate;
        targetDamageable.OnDie += HandleDeath;
        fillImage.fillAmount = Mathf.Clamp01(targetDamageable.Percent);
    }

    private void OnDisable()
    {
        if (targetDamageable != null)
        {
            targetDamageable.OnHit -= HandleHitUpdate;
            targetDamageable.OnDie -= HandleDeath;
        }

        StopTweens();
        RectTransform rect = transform as RectTransform;
        if (rect != null)
        {
            rect.localPosition = baseLocalPosition;
            rect.localScale = baseLocalScale;
        }
    }

    private void HandleHitUpdate(int damage)
    {
        if (fillImage == null || targetDamageable == null) return;

        if (healthTween.isAlive) healthTween.Stop();
        if (shakeTween.isAlive) shakeTween.Stop();

        float targetPercent = targetDamageable.Percent - damage / (float)Mathf.Max(1, targetDamageable.maxHP);
        healthTween = Tween.UIFillAmount(fillImage, Mathf.Clamp01(targetPercent), duration, easeType);

        RectTransform rect = transform as RectTransform;
        if (rect != null)
        {
            shakeTween = Tween.ShakeLocalPosition(rect, strength: new Vector3(6f, 4f, 0f), duration: 0.15f);
        }
    }

    private void HandleDeath()
    {
        if (healthTween.isAlive) healthTween.Stop();
        if (deathTween.isAlive) deathTween.Stop();

        RectTransform rect = transform as RectTransform;
        if (rect != null)
        {
            deathTween = Tween.Scale(rect, Vector3.zero, 0.2f, Ease.InBack)
                .OnComplete(() =>
                {
                    if (gameObject != null) gameObject.SetActive(false);
                });
        }
    }

    private void StopTweens()
    {
        if (healthTween.isAlive) healthTween.Stop();
        if (shakeTween.isAlive) shakeTween.Stop();
        if (deathTween.isAlive) deathTween.Stop();
    }
}
