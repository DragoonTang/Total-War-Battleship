using UnityEngine;

public class PauseTrigger : MonoBehaviour
{
    // 静态计数器：所有 UI 实例共用
    private static int pauseLayerCount = 0;

    // 静态快照：记录第一层 UI 打开前的 timeScale
    private static float originalTimeScale = 1.0f;

    private void OnEnable()
    {
        // 如果是打开的第一个暂停 UI，记录当前的原始速率
        if (pauseLayerCount == 0)
        {
            originalTimeScale = Time.timeScale;
        }

        pauseLayerCount++;
        UpdateTimeScale();
    }

    private void OnDisable()
    {
        pauseLayerCount--;
        if (pauseLayerCount < 0) pauseLayerCount = 0;

        UpdateTimeScale();
    }

    private void OnDestroy()
    {
        // 场景销毁时，直接暴力恢复原始速率并清零计数器
        if (pauseLayerCount > 0)
        {
            pauseLayerCount = 0;
            Time.timeScale = 1.0f; // 切换场景通常建议回归标准 1.0
        }
    }

    private void UpdateTimeScale()
    {
        if (pauseLayerCount > 0)
        {
            // 只要有 UI 开启，就进入静止
            Time.timeScale = 0f;
        }
        else
        {
            // 全部关闭后，恢复到进入暂停前的那个速率
            Time.timeScale = originalTimeScale;
        }
    }
}