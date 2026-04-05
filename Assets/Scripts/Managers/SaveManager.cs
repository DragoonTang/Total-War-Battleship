using UnityEngine;

public enum LanguageType
{
    None = -1,
    Chinese = 0,
    Japanese = 1,
    English = 2,
}

public static class SaveManager
{
    private const string KEY_LANGUAGE = "UserSelectedLanguage";

    // 接收枚举类型，增加代码安全性 🛡️
    public static void SaveLanguage(LanguageType lang)
    {
        PlayerPrefs.SetInt(KEY_LANGUAGE, (int)lang);
        PlayerPrefs.Save();
    }

    // 读取时将 int 转回枚举
    public static LanguageType LoadLanguage()
    {
        // 如果没有存档，默认返回 -1 (None)
        int savedVal = PlayerPrefs.GetInt(KEY_LANGUAGE, -1);
        return (LanguageType)savedVal;
    }
}