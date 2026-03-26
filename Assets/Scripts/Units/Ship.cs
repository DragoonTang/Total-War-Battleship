using UnityEngine;

/// <summary>
/// 🐉 舰船动力与物理控制
/// </summary>
public class Ship : MonoBehaviour
{
    [Header("移动参数")]
    public float maxSpeed = 10f;
    public float acceleration = 2f; // 加速度
    public float turnSpeed = 50f;   // 转向速度

    [Header("当前状态")]
    [SerializeField] private float targetSpeed = 0f;  // 目标档位速度
    [SerializeField] private float currentSpeed = 0f; // 当前实际速度
    [SerializeField] private float turnDirection = 0f; // 舵向指令: -1, 0, 1

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 增加阻尼感，使船只停下时更自然
        if (rb != null)
        {
            rb.linearDamping = 0.5f;
            rb.angularDamping = 1.0f;
        }
    }

    void FixedUpdate() // 物理计算建议在 FixedUpdate 中进行
    {
        // 1. 持续向目标档位速度平滑过渡
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);

        // 2. 应用线速度
        rb.linearVelocity = transform.forward * currentSpeed;

        // 3. 应用角速度（模拟舵效：船速越快，转向越明显）
        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed+0.1f) / 1.0f);
        float finalTurn = turnDirection * turnSpeed * speedFactor;

        rb.angularVelocity = new Vector3(0, finalTurn * Mathf.Deg2Rad, 0);
    }

    /// <summary>
    /// 由 UIManager 调用：通过档位名称解析出的数值设置目标速度
    /// </summary>
    public void SetSpeed(float telegraph)
    {
        targetSpeed = telegraph * maxSpeed;
    }

    /// <summary>
    /// 由 UIManager 调用：锁定舵角方向
    /// </summary>
    public void SetTurnDirection(int direction)
    {
        turnDirection = direction;
    }
}