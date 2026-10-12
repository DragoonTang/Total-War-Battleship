using UnityEngine;

/// <summary>窄屏时等比缩小横栏内容；屏幕边缘仍由父节点的 Stretch 锚点决定。</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class BottomHudLayout : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField, Min(1f)] private float minimumContentWidth = 1450f;
    [SerializeField, Min(1f)] private float contentHeight = 260f;

    private void OnEnable() => Refresh();
    private void OnRectTransformDimensionsChange() => Refresh();
    private void LateUpdate() => Refresh();

    private void Refresh()
    {
        if (!Application.IsPlaying(gameObject) || content == null) return;
        float width = ((RectTransform)transform).rect.width;
        if (width <= 0f) return;
        float scale = Mathf.Min(1f, width / Mathf.Max(1f, minimumContentWidth));
        // 内容本身也是横向 Stretch；仅在窄屏增加等比缩放所需的布局宽度。
        Vector2 size = new(width / scale - width, contentHeight);
        Vector3 contentScale = new(scale, scale, 1f);
        if (content.sizeDelta != size) content.sizeDelta = size;
        if (content.localScale != contentScale) content.localScale = contentScale;
    }
}
