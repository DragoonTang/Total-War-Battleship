using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [SerializeField] LanguagePanel panelLang;
    [SerializeField] float duration = 1.6f;

    [SerializeField] Button btnStart;        // 原来的全屏按钮
    [SerializeField] Text btnText;           // 闪烁文字
    [SerializeField] GameObject mainMenuRoot; // 新战役/继续战役按钮的父物体

    [SerializeField] Button btnNewGame;
    [SerializeField] Button btnContinue;

    private void Awake()
    {
        panelLang.CheckInitialLanguage();
    }

    private void Start()
    {
        // 闪烁效果保持不变
        Tween.Alpha(btnText, 1f, 0.2f, duration,
            cycles: -1,
            cycleMode: CycleMode.Yoyo,
            ease: Ease.InOutSine);

        btnStart.onClick.AddListener(OnTapAnywhere);
        btnNewGame.onClick.AddListener(OnNewGameClicked);
        btnContinue.onClick.AddListener(OnContinueClicked);

        mainMenuRoot.SetActive(false);
    }

    private void OnTapAnywhere()
    {
        // 停止闪烁
        Tween.StopAll(this);

        // 隐藏闪烁文字
        btnText.gameObject.SetActive(false);

        // 显示菜单按钮
        mainMenuRoot.SetActive(true);
    }

    private void OnNewGameClicked()
    {
        GameManager.IsNew = true;
        SceneManager.LoadScene(1);
    }

    private void OnContinueClicked()
    {
        GameManager.IsNew = false;
        SceneManager.LoadScene(1);
    }
}
