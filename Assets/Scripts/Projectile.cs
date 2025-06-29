using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public float downwardForce = 9.81f; // 可自定义“重力感”

    private Rigidbody rb;
    private float timer;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        rb.linearVelocity = transform.forward * speed;
        //rb.useGravity = true;
        //Physics.gravity = Vector3.down * 9.81f * gravityScale;

        timer = lifetime;
    }

    void FixedUpdate()
    {
        // 施加一个持续向下的自定义力（仅影响 Y 轴）
        rb.linearVelocity += Vector3.down * downwardForce * Time.fixedDeltaTime;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Despawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
       
    }

    void Despawn()
    {

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        SimplePool.Despawn(gameObject);
    }
}
