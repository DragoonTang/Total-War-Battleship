using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HudPressFeedback : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform pulseVisual;
    [SerializeField] private Image pulseImage;
    [SerializeField] private float duration = 0.18f;

    private Tween scaleTween;
    private Tween fadeTween;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pulseVisual == null || pulseImage == null) return;

        StopTweens();
        pulseVisual.gameObject.SetActive(true);
        pulseVisual.localScale = Vector3.one * 0.9f;
        Color color = pulseImage.color;
        color.a = 0.42f;
        pulseImage.color = color;

        scaleTween = Tween.Scale(pulseVisual, Vector3.one * 1.08f, duration, Ease.OutQuad, useUnscaledTime: true);
        fadeTween = Tween.Alpha(pulseImage, 0f, duration, Ease.OutQuad, useUnscaledTime: true)
            .OnComplete(() =>
            {
                if (pulseVisual != null) pulseVisual.gameObject.SetActive(false);
            });
    }

    private void OnDisable()
    {
        StopTweens();
        if (pulseVisual != null)
        {
            pulseVisual.localScale = Vector3.one;
            pulseVisual.gameObject.SetActive(false);
        }
    }

    private void StopTweens()
    {
        if (scaleTween.isAlive) scaleTween.Stop();
        if (fadeTween.isAlive) fadeTween.Stop();
    }
}
