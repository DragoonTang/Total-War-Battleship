using UnityEngine;
using UnityEngine.UI;
using PrimeTween; // 使用 PrimeTween 处理动画
using UnityEngine.SceneManagement; // 场景加载所需

/// <summary>
/// 🐉 开始界面控制器
/// </summary>
public class StartPanel : MonoBehaviour
{
    [SerializeField,Tooltip("语言面板")] LanguagePanel panelLang;
    [SerializeField] float duration = 1.6f; // 闪烁周期
    private Button btnStart;
    private Text btnText;

    private void Awake()
    {
        btnStart = GetComponentInChildren<Button>();

        btnText = btnStart.GetComponentInChildren<Text>();

        panelLang.CheckInitialLanguage();
    }

    private void Start()
    {
        // 1. 实现按钮文字不断闪烁
        if (btnText != null)
        {
            Tween.Alpha(btnText, 1f, 0.2f, duration,
             cycles: -1,             // -1 代表无限循环
             cycleMode: CycleMode.Yoyo, // 来回往复
             ease: Ease.InOutSine    // 缓动
             );
        }

        // 2. 绑定点击事件，加载场景 1
        btnStart.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        // 停止当前物体的所有动画（防止加载过程中出现冗余行为）
        Tween.StopAll(this);

        // 加载到场景 1
        SceneManager.LoadScene(1);
    }
}