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

    public Button btnSpeed1, btnSpeed2, btnSpeed3, btnSpeed4;
    public Button btnTurnLeft, btnTurnRight, btnTurnCenter;
    public Button btnReverse;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        btnSpeed1.onClick.AddListener(() => SetSpeed(maxSpeed * 0.25f));
        btnSpeed2.onClick.AddListener(() => SetSpeed(maxSpeed * 0.5f));
        btnSpeed3.onClick.AddListener(() => SetSpeed(maxSpeed * 0.75f));
        btnSpeed4.onClick.AddListener(() => SetSpeed(maxSpeed));

        btnReverse.onClick.AddListener(() => SetSpeed(-maxSpeed * 0.5f));

        btnTurnLeft.onClick.AddListener(() => turnDirection = -1);
        btnTurnRight.onClick.AddListener(() => turnDirection = 1);
        btnTurnCenter.onClick.AddListener(() => turnDirection = 0);
    }

    void Update()
    {
        rb.linearVelocity = transform.forward * currentSpeed;
        rb.angularVelocity = new Vector3(0, turnDirection * turnSpeed * Time.deltaTime, 0);
    }

    private void SetSpeed(float targetSpeed)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);
    }

}
