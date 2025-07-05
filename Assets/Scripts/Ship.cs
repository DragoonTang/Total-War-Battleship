using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 🐉
/// </summary>
public class Ship : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 2f;
    public float turnSpeed = 50f;
    private float currentSpeed = 0f;
    private float turnDirection = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.linearVelocity = transform.forward * currentSpeed;
        rb.angularVelocity = new Vector3(0, turnDirection * turnSpeed * Time.deltaTime, 0);
    }

    /// <summary>
    /// 档位速度设置
    /// </summary>
    /// <param name="telegraph"></param>
    public void SetSpeed(float telegraph)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, telegraph*maxSpeed, Time.deltaTime * acceleration);
    }

    public void SetTurnDirection(int direction)
    {
        turnDirection = direction;
    }
}
