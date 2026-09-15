using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStep
    {
        WaitingForStart,
        Turn,
        StopTurn,
        Forward,
        StopShip,
        Reverse,
        ResumeForward,
        MainGun,
        Torpedo,
        Health,
        Complete,
        Finished
    }

    private const string TableName = "MainUI";

    [Header("Tutorial UI")]
    [SerializeField] private TutorialUI tutorialUI;

    [Header("Rudder")]
    [SerializeField] private Toggle rudderLeft;
    [SerializeField] private Toggle rudderStop;
    [SerializeField] private Toggle rudderRight;

    [Header("Telegraph")]
    [SerializeField] private Toggle[] forwardGears;
    [SerializeField] private Toggle stopShip;
    [SerializeField] private Toggle reverse;

    [Header("Combat UI")]
    [SerializeField] private TorpedoWeaponUI torpedoWeaponUI;
    [SerializeField] private RectTransform healthDisplay;

    [Header("Timing")]
    [SerializeField] private float completionDisplaySeconds = 1.75f;

    [SerializeField] private TutorialStep currentStep = TutorialStep.WaitingForStart;
    private Coroutine completionRoutine;

    public TutorialStep CurrentStep => currentStep;
    public bool IsRunning => currentStep != TutorialStep.WaitingForStart && currentStep != TutorialStep.Finished;

    private void Start()
    {
        if (tutorialUI != null)
        {
            tutorialUI.Hide();
            tutorialUI.ContinueButton.onClick.AddListener(OnContinueClicked);
        }

        AddToggleListener(rudderLeft, OnRudderChanged);
        AddToggleListener(rudderRight, OnRudderChanged);
        AddToggleListener(rudderStop, OnRudderStopChanged);
        AddToggleListener(stopShip, OnStopShipChanged);
        AddToggleListener(reverse, OnReverseChanged);

        if (forwardGears != null)
        {
            foreach (Toggle gear in forwardGears) AddToggleListener(gear, OnForwardGearChanged);
        }

        if (torpedoWeaponUI != null) torpedoWeaponUI.OnFired += OnTorpedoFired;
    }

    private void OnDestroy()
    {
        if (tutorialUI != null && tutorialUI.ContinueButton != null)
            tutorialUI.ContinueButton.onClick.RemoveListener(OnContinueClicked);

        RemoveToggleListener(rudderLeft, OnRudderChanged);
        RemoveToggleListener(rudderRight, OnRudderChanged);
        RemoveToggleListener(rudderStop, OnRudderStopChanged);
        RemoveToggleListener(stopShip, OnStopShipChanged);
        RemoveToggleListener(reverse, OnReverseChanged);

        if (forwardGears != null)
        {
            foreach (Toggle gear in forwardGears) RemoveToggleListener(gear, OnForwardGearChanged);
        }

        if (torpedoWeaponUI != null) torpedoWeaponUI.OnFired -= OnTorpedoFired;
    }

    public void BeginTutorial()
    {
        if (currentStep != TutorialStep.WaitingForStart || tutorialUI == null) return;

        tutorialUI.SetStaticText(Localize("ui_tutorial_title"), Localize("ui_tutorial_continue"));
        ShowStep(TutorialStep.Turn);
    }

    private static void AddToggleListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> listener)
    {
        if (toggle != null) toggle.onValueChanged.AddListener(listener);
    }

    private static void RemoveToggleListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> listener)
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(listener);
    }

    private void OnRudderChanged(bool isOn)
    {
        if (isOn && currentStep == TutorialStep.Turn) ShowStep(TutorialStep.StopTurn);
    }

    private void OnRudderStopChanged(bool isOn)
    {
        if (isOn && currentStep == TutorialStep.StopTurn) ShowStep(TutorialStep.Forward);
    }

    private void OnForwardGearChanged(bool isOn)
    {
        if (!isOn) return;

        if (currentStep == TutorialStep.Forward)
            ShowStep(TutorialStep.StopShip);
        else if (currentStep == TutorialStep.ResumeForward)
            ShowStep(TutorialStep.MainGun);
    }

    private void OnStopShipChanged(bool isOn)
    {
        if (isOn && currentStep == TutorialStep.StopShip) ShowStep(TutorialStep.Reverse);
    }

    private void OnReverseChanged(bool isOn)
    {
        if (isOn && currentStep == TutorialStep.Reverse) ShowStep(TutorialStep.ResumeForward);
    }

    private void OnTorpedoFired()
    {
        if (currentStep == TutorialStep.Torpedo) ShowStep(TutorialStep.Health);
    }

    private void OnContinueClicked()
    {
        if (currentStep == TutorialStep.MainGun)
            ShowStep(TutorialStep.Torpedo);
        else if (currentStep == TutorialStep.Health)
            ShowStep(TutorialStep.Complete);
    }

    private void ShowStep(TutorialStep step)
    {
        currentStep = step;

        switch (step)
        {
            case TutorialStep.Turn:
                tutorialUI.Show(Localize("ui_tutorial_turn"), Rects(rudderLeft, rudderRight), false);
                break;
            case TutorialStep.StopTurn:
                tutorialUI.Show(Localize("ui_tutorial_stop_turn"), Rects(rudderStop), false);
                break;
            case TutorialStep.Forward:
                tutorialUI.Show(Localize("ui_tutorial_forward"), Rects(forwardGears), false);
                break;
            case TutorialStep.StopShip:
                tutorialUI.Show(Localize("ui_tutorial_stop_ship"), Rects(stopShip), false);
                break;
            case TutorialStep.Reverse:
                tutorialUI.Show(Localize("ui_tutorial_reverse"), Rects(reverse), false);
                break;
            case TutorialStep.ResumeForward:
                tutorialUI.Show(Localize("ui_tutorial_resume_forward"), Rects(forwardGears), false);
                break;
            case TutorialStep.MainGun:
                tutorialUI.Show(Localize("ui_tutorial_main_gun"), null, true);
                break;
            case TutorialStep.Torpedo:
                tutorialUI.Show(Localize("ui_tutorial_torpedo"), Rects(torpedoWeaponUI == null ? null : torpedoWeaponUI.fireButton), false);
                break;
            case TutorialStep.Health:
                tutorialUI.Show(Localize("ui_tutorial_health"), Rects(healthDisplay), true);
                break;
            case TutorialStep.Complete:
                tutorialUI.Show(Localize("ui_tutorial_complete"), null, false);
                if (completionRoutine != null) StopCoroutine(completionRoutine);
                completionRoutine = StartCoroutine(FinishAfterDelay());
                break;
        }
    }

    private IEnumerator FinishAfterDelay()
    {
        yield return new WaitForSecondsRealtime(completionDisplaySeconds);
        tutorialUI.Hide();
        currentStep = TutorialStep.Finished;
        completionRoutine = null;
    }

    private static string Localize(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key);
    }

    private static IReadOnlyList<RectTransform> Rects(params Component[] components)
    {
        var result = new List<RectTransform>();
        if (components == null) return result;

        foreach (Component component in components)
        {
            if (component != null && component.transform is RectTransform rect) result.Add(rect);
        }
        return result;
    }

    private static IReadOnlyList<RectTransform> Rects(Toggle[] toggles)
    {
        var result = new List<RectTransform>();
        if (toggles == null) return result;

        foreach (Toggle toggle in toggles)
        {
            if (toggle != null && toggle.transform is RectTransform rect) result.Add(rect);
        }
        return result;
    }
}
