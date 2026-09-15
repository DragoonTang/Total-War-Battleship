using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text messageText;
    [SerializeField] private Text continueButtonText;

    [Header("Controls")]
    [SerializeField] private Button continueButton;

    [Header("Highlight")]
    [SerializeField] private RectTransform highlightLayer;
    [SerializeField] private RectTransform highlightTemplate;
    [SerializeField] private float highlightPadding = 14f;

    private readonly List<RectTransform> highlightFrames = new();
    private readonly List<RectTransform> currentTargets = new();
    private Canvas rootCanvas;

    public Button ContinueButton => continueButton;

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (highlightTemplate != null)
        {
            highlightTemplate.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        UpdateHighlightPositions();
    }

    public void SetStaticText(string title, string continueLabel)
    {
        if (titleText != null) titleText.text = title;
        if (continueButtonText != null) continueButtonText.text = continueLabel;
    }

    public void Show(string message, IReadOnlyList<RectTransform> targets, bool showContinue)
    {
        gameObject.SetActive(true);
        if (messageText != null) messageText.text = message;
        if (continueButton != null) continueButton.gameObject.SetActive(showContinue);

        currentTargets.Clear();
        if (targets != null)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] != null) currentTargets.Add(targets[i]);
            }
        }

        EnsureHighlightCount(currentTargets.Count);
        UpdateHighlightPositions();
    }

    public void Hide()
    {
        currentTargets.Clear();
        SetHighlightsActive(0);
        gameObject.SetActive(false);
    }

    private void EnsureHighlightCount(int count)
    {
        if (highlightTemplate == null || highlightLayer == null)
        {
            SetHighlightsActive(0);
            return;
        }

        while (highlightFrames.Count < count)
        {
            RectTransform frame = Instantiate(highlightTemplate, highlightLayer);
            frame.name = "HighlightFrame";
            foreach (Graphic graphic in frame.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }
            highlightFrames.Add(frame);
        }

        SetHighlightsActive(count);
    }

    private void SetHighlightsActive(int activeCount)
    {
        for (int i = 0; i < highlightFrames.Count; i++)
        {
            highlightFrames[i].gameObject.SetActive(i < activeCount);
        }
    }

    private void UpdateHighlightPositions()
    {
        if (highlightLayer == null || currentTargets.Count == 0) return;

        Camera eventCamera = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = rootCanvas.worldCamera;
        }

        var corners = new Vector3[4];
        for (int i = 0; i < currentTargets.Count && i < highlightFrames.Count; i++)
        {
            RectTransform target = currentTargets[i];
            RectTransform frame = highlightFrames[i];
            if (target == null || frame == null) continue;

            target.GetWorldCorners(corners);
            Vector2 min = new(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new(float.NegativeInfinity, float.NegativeInfinity);

            for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(eventCamera, corners[cornerIndex]);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(highlightLayer, screenPoint, eventCamera, out Vector2 localPoint);
                min = Vector2.Min(min, localPoint);
                max = Vector2.Max(max, localPoint);
            }

            frame.anchoredPosition = (min + max) * 0.5f;
            frame.sizeDelta = max - min + Vector2.one * (highlightPadding * 2f);
        }
    }
}
