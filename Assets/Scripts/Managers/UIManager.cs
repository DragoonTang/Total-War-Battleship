using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 🐉 操纵杆管理器：将 UI 的 Toggle 状态转换为船只指令
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    /// <summary>
    /// 退出
    /// </summary>
    [SerializeField] Button btnExit;
    [SerializeField] LanguagePanel panelLang;

    [Header("核心引用")]
    [SerializeField] private Ship playerShip; // 在 Inspector 中拖入玩家的 Ship 物体

    [Header("动力组 (Toggle Group)")]
    [SerializeField] private ToggleGroup speedGroup;

    [Header("舵向组 (Toggle Group)")]
    [SerializeField] private ToggleGroup turnGroup;

    [Header("音量控制 (UI)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;

    private void Start()
    {
        // 1. 初始化动力组逻辑
        InitGroup(speedGroup, (val) =>
        {
            playerShip.SetSpeed(val);
        });

        // 2. 初始化舵向组逻辑
        InitGroup(turnGroup, (val) =>
        {
            playerShip.SetTurnDirection((int)val);
        });

        btnExit.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayUISound("UI_Click");
            GameManager.ExitGame();
        });

        // 音频 UI 绑定
        var audio = AudioManager.Instance;
        if (audio != null)
        {
            if (musicSlider != null)
            {
                musicSlider.onValueChanged.AddListener((v) => audio.SetMusicVolumeFromLinear(v));
                musicSlider.value = audio.GetMusicLinearVolume();
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener((v) => audio.SetSFXVolumeFromLinear(v));
                sfxSlider.value = audio.GetSFXLinearVolume();
            }

            if (musicMuteToggle != null)
            {
                musicMuteToggle.onValueChanged.AddListener((v) => audio.SetMusicMute(v));
                musicMuteToggle.isOn = false; // 默认不静音
            }

            if (sfxMuteToggle != null)
            {
                sfxMuteToggle.onValueChanged.AddListener((v) => audio.SetSFXMute(v));
                sfxMuteToggle.isOn = false;
            }
        }

        // 检查语言面板显示
        panelLang.CheckInitialLanguage();
    }

    /// <summary>
    /// 遍历组内的所有 Toggle，根据物体名称绑定逻辑
    /// </summary>
    private void InitGroup(ToggleGroup group, System.Action<float> onAction)
    {
        if (group == null) return;

        // 获取该组下所有的 Toggle
        Toggle[] toggles = group.GetComponentsInChildren<Toggle>();

        foreach (var t in toggles)
        {
            // 关键：我们将数值写在物体的名字上，例如 "1", "0.5", "-1"
            if (float.TryParse(t.gameObject.name, out float value))
            {
                // 绑定状态改变事件
                t.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn) // 只有当被选中时才触发指令
                    {
                        onAction.Invoke(value);
                        // 播放 UI 点击音效
                        AudioManager.Instance.PlayUISound("UI_Click");
                    }
                });

                // 初始检查：如果当前 Toggle 默认就是开启的，立即执行一次指令
                if (t.isOn) onAction.Invoke(value);
            }
            else
            {
                Debug.LogWarning($"UIManager: 物体 {t.name} 的名称无法解析为有效数值！");
            }
        }
    }

    // 原有的弹出消息接口保留，以后可能用到
    public void ShowPopMessage(string title, string content) { }
}