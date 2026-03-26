using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    int damage = 10; // 伤害值
    public float speed = 20f;
    public float lifetime = 5f;
    public float downwardForce = 5f; // 可自定义“重力感”
    [SerializeField] GameObject effect;

    /// <summary>
    /// 发射者，不能被该发射物伤害
    /// </summary>
    Transform shooter;
    private Rigidbody rb;
    private float timer;
    private TrailRenderer trailRenderer;
    /// <summary>
    /// 阵营
    /// </summary>
    private bool isEnemy;

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
        if (timer < 2)
            rb.linearVelocity += Vector3.down * downwardForce * Time.fixedDeltaTime;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Despawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 穿过发射者时不发生碰撞
        if (other.transform.root == shooter)
            return;

        if (other.TryGetComponent(out Damageable target))
        {
            // 阵营不同则伤害目标，同阵营则无伤害
            if (target.isEnemy != isEnemy)
                target.TakeDamage(damage);

            SimplePool.Instance.Spawn(effect, transform.position, Quaternion.identity).transform.parent= target.transform;
            Despawn();
        }
        // 碰撞地面或墙壁，直接销毁
        else if (LayerMask.LayerToName(other.gameObject.layer) == "Water")
        {
            Despawn();
        }
    }

    public void Initialize(bool side, Transform owner)
    {
        isEnemy = side;
        shooter = owner;
    }

    void Despawn()
    {

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SimplePool.Instance.Despawn(gameObject);
    }
}
