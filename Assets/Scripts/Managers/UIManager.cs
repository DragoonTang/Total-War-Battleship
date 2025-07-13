using UnityEngine;

/// <summary>
/// 🐉
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    Canvas canvas; // 画布组件

    private void Start()
    {
        if (canvas == null) {
            canvas = FindFirstObjectByType<Canvas>();
        }
    }

    public void ShowPopMessage(string title,string content) {
    
    }
}
