using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AttackRangeVisualizer : MonoBehaviour
{
    [SerializeField]
    private Turret parentTurret; // 关联底座上的炮塔脚本

    public int segments = 50; // 增加分段使弧线更圆滑
    public Color lineColor = new Color(0, 1, 0, 0.3f); // 建议玩家使用绿色半透明

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // 关键：设为 false，扇形将相对于 turret_base 本身
        lineRenderer.useWorldSpace = false;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.08f;
        lineRenderer.startColor = lineRenderer.endColor = lineColor;

        // 自动获取同级的 Turret 脚本
        if (parentTurret == null) parentTurret = GetComponent<Turret>();

        if (parentTurret != null)
        {
            // 绘制由底座定义的物理极限范围
            DrawSector(parentTurret.maxRange, parentTurret.rotationLimitAngle);
        }
    }

    // 如果你在编辑器里实时调参数，可以用 OnValidate，否则 Start 运行一次即可
    void DrawSector(float radius, float angle)
    {
        lineRenderer.positionCount = segments + 2;
        float halfAngle = angle * 0.5f;
        float angleStep = angle / segments;

        lineRenderer.SetPosition(0, Vector3.zero); // 圆心在底座坐标原点

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * currentAngle;

            // 保持以底座 Forward 为中心的扇形
            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;

            lineRenderer.SetPosition(i + 1, new Vector3(x, 0, z));
        }
    }
}