using UnityEngine;

/// <summary>
/// 🐉 全局游戏管理器（镜头控制已移至 CinemachineCameraControl）
/// </summary>
public class GameManager : MonoBehaviour
{
    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
