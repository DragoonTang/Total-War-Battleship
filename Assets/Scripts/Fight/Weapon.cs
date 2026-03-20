using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isEnemy;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [HideInInspector] public float radius; // 由 Turret 脚本在 Start 时同步过来
    public float fireRate = 2f;
    public float fireThreshold = 5f; // 开火容差角度

    private float nextFireTime;
    private Transform currentTarget;

    // 由父级 Turret 调用，告知当前目标
    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    void Update()
    {
        if (currentTarget != null && CanFire())
        {
            Fire();
        }
    }

    bool CanFire()
    {
        if (Time.time < nextFireTime) return false;

        // 判定：炮口正前方和目标的夹角
        Vector3 toEnemy = (currentTarget.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toEnemy);

        // 距离判定（双重保险）
        float dist = Vector3.Distance(transform.position, currentTarget.position);

        return angle <= fireThreshold && dist <= radius;
    }

    void Fire()
    {
        nextFireTime = Time.time + fireRate;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // 这里的 Projectile 逻辑保持不变
        if (bullet.TryGetComponent<Projectile>(out var p)) p.isEnemy = this.isEnemy;
    }
}