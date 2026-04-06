using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguagePanel : MonoBehaviour
{
    [Header("UI References")]
    public Button btnChinese;
    public Button btnJapanese;
    public Button btnEnglish;
    public Button btnConfirm;

    // 记录用户当前在面板上点选的临时语言
    private LanguageType _tempSelected ;

    private void Start()
    {
        // 绑定按钮点击事件
        btnChinese.onClick.AddListener(() => OnLanguageBtnClick(LanguageType.Chinese));
        btnJapanese.onClick.AddListener(() => OnLanguageBtnClick(LanguageType.Japanese));
        btnEnglish.onClick.AddListener(() => OnLanguageBtnClick(LanguageType.English));
        btnConfirm.onClick.AddListener(OnConfirmClick);
    }

    /// <summary>
    /// 由UI管理器来检查存档决定是否显示
    /// </summary>
    public void CheckInitialLanguage()
    {
        LanguageType savedLang = SaveManager.LoadLanguage();

        if (savedLang == LanguageType.None)
        {
            gameObject.SetActive(true);

            // 获取系统默认语言
            LanguageType autoDetected = GetSystemDefaultLanguage();
            Debug.Log("识别系统语言..."+ autoDetected);

            // 自动触发一次点击逻辑，实现预览和选中状态
            OnLanguageBtnClick(autoDetected);
        }
        else
        {
            SetLanguage(savedLang);
        }
    }

    // 处理语言按钮点击（预览模式）
    private void OnLanguageBtnClick(LanguageType type)
    {
        _tempSelected = type;
        SetLanguage(type); // 实时预览效果
        Debug.Log($"临时选择: {type}");
    }

    // 处理确认按钮点击（正式保存）
    private void OnConfirmClick()
    {
        if (_tempSelected != LanguageType.None)
        {
            SaveManager.SaveLanguage(_tempSelected);
            gameObject.SetActive(false); // 关闭面板
            Debug.Log($"已保存语言: {_tempSelected}");
        }
    }

    private void SetLanguage(LanguageType type)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)type];
    }

    private LanguageType GetSystemDefaultLanguage()
    {
        // 获取 Unity 识别到的设备系统语言
        SystemLanguage lang = Application.systemLanguage;

        switch (lang)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                return LanguageType.Chinese; // 对应枚举 0

            case SystemLanguage.Japanese:
                return LanguageType.Japanese; // 对应枚举 1

            default:
                // 如果是英语或其他不支持的语言，统一返回英语
                return LanguageType.English; // 对应枚举 2
        }
    }
}