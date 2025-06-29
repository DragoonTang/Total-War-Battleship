using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AttackRangeVisualizer : MonoBehaviour
{
    [SerializeField]
    Weapon weapon; // 关联的武器脚本（如果需要）

    public float radius = 5f;
    public float angle = 60f;
    public int segments = 30;
    public Color lineColor = new Color(1, 0, 0, 0.5f);

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.loop = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = segments + 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.startColor = lineRenderer.endColor = lineColor;

        if (weapon != null)
        {
            radius = weapon.radius; // 从武器脚本获取攻击范围
            angle = weapon.angle; // 从武器脚本获取攻击角度
        }

        DrawSector();
    }

    void Update()
    {
        DrawSector(); // 实时刷新以追踪旋转
    }

    void DrawSector()
    {
        float halfAngle = angle * 0.5f;
        float angleStep = angle / segments;

        lineRenderer.SetPosition(0, Vector3.zero);
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * currentAngle;
            Vector3 pos = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;
            lineRenderer.SetPosition(i + 1, pos);
        }
    }
}
