using UnityEngine;
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
        text.text = isWin ? "You Win!" : "Game Over!";
    }
}
