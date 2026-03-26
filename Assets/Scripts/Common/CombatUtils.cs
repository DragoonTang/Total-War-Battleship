using UnityEngine;

public static class CombatUtils
{
    /// <summary>
    /// 计算点到 BoxCollider 在 XZ 平面投影矩形的平方距离
    /// 核心逻辑：将点转入本地空间，消除船只旋转对计算的影响
    /// </summary>
    public static float SqrDistanceToBox(Vector3 observerPos, BoxCollider targetBox)
    {
        if (targetBox == null) return float.MaxValue;

        // 1. 将观察者位置转为目标盒子的本地坐标
        Vector3 localPoint = targetBox.transform.InverseTransformPoint(observerPos);

        // 2. 获取盒子在本地空间下的半长宽（取 X 和 Z，忽略 Y 轴高度）
        // 乘以 lossyScale 是为了兼容那些被缩放过的船只模型
        Vector3 boxSize = targetBox.size;
        float halfW = (boxSize.x * targetBox.transform.lossyScale.x) * 0.5f;
        float halfL = (boxSize.z * targetBox.transform.lossyScale.z) * 0.5f;

        // 3. 计算本地坐标系下，点到矩形边缘的偏移量（dx, dz）
        // 如果点在矩形内，偏移量为 0
        float dx = Mathf.Max(0, Mathf.Abs(localPoint.x) - halfW);
        float dz = Mathf.Max(0, Mathf.Abs(localPoint.z) - halfL);

        // 4. 返回平方距离（dx^2 + dz^2），无需开方，性能最高
        return dx * dx + dz * dz;
    }

    /// <summary>
    /// 平面角度判定：判断目标是否在来源物体的正前方一定夹角内
    /// </summary>
    public static bool IsInAngle(Transform origin, Vector3 targetPos, float thresholdAngle)
    {
        Vector3 targetDir = targetPos - origin.position;
        targetDir.y = 0; // 强制平面化

        if (targetDir == Vector3.zero) return true;

        // 计算来源物体正前方与目标方向的夹角
        float angle = Vector3.Angle(origin.forward, targetDir.normalized);
        return angle <= thresholdAngle;
    }

    // --- 以下是保留的旧方法，如果你的其他脚本没用到，可以删掉 ---

    public static bool IsInRange(Vector3 origin, Vector3 target, float range)
    {
        return (target - origin).sqrMagnitude <= (range * range);
    }
}