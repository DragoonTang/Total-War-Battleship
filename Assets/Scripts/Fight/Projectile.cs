using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    int damage = 10; // 伤害值
    public float speed = 20f;
    public float lifetime = 5f;
    public float downwardForce = 9.81f; // 可自定义“重力感”
    /// <summary>
    /// 发射者，不能被该发射物伤害
    /// </summary>
    public Damageable shooter;
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
        if (other.TryGetComponent(out Damageable target) && target != shooter)
        {
            // 伤害目标
            target.TakeDamage(damage);
            Despawn();
        }
        else if (LayerMask.LayerToName(other.gameObject.layer) == "Water")
        {
            // 碰撞地面或墙壁，直接销毁
            Despawn();
        }
    }

    void Despawn()
    {

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SimplePool.Despawn(gameObject);
    }
}
