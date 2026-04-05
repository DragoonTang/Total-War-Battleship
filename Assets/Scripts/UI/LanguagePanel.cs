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
            // 如果没选过，保持面板显示
            Debug.Log("首次启动，请选择语言");
            gameObject.SetActive(true);
        }
        else
        {
            // 如果已有存档，直接设置语言
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
}