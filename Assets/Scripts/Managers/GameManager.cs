using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 🐉 全局游戏管理器（镜头控制已移至 CinemachineCameraControl）
/// </summary>
public class GameManager : MonoBehaviour
{
    public static void RestartLevel()
    {
        // 直接重载当前场景，保持所有静态数据不变
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
