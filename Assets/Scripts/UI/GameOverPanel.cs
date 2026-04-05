using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField]
    Text text;

    private void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(() => SceneManager.LoadScene(0));
    }

    public void Show(bool isWin)
    {
        var key = isWin ? "ui_win" : "ui_lose";
        text.text = LocalizationSettings.StringDatabase.GetLocalizedString("MainUI", key);
    }
}
