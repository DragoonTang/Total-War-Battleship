using UnityEngine;

public class Turret : MonoBehaviour
{
    public bool isEnemy;
    public Transform gunNode; // 拖入你的 "Gun" 节点

    [Header("性能参数")]
    public float maxRange = 20f;
    public float rotationLimitAngle = 120f; // 炮塔左右各 60 度的射界
    public float rotateSpeed = 90f;

    private Weapon childWeapon;
    private Transform currentTarget;

    void Start()
    {
        childWeapon = GetComponentInChildren<Weapon>();
        if (childWeapon != null)
        {
            childWeapon.isEnemy = this.isEnemy;
            childWeapon.radius = this.maxRange; // 强制同步射程
        }
    }

    void Update()
    {
        SearchTarget();
        if (currentTarget != null)
        {
            RotateTowardsTarget();
            if (childWeapon != null) childWeapon.SetTarget(currentTarget);
        }
        else
        {
            if (childWeapon != null) childWeapon.SetTarget(null);
        }
    }

    void SearchTarget()
    {
        // 沿用之前的索敌逻辑，但结果留在 Turret 层
        var enemies = isEnemy ? BattleSceneController.Instance.playerUnits : BattleSceneController.Instance.enemyUnits;
        float minDistance = maxRange;
        Transform bestTarget = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            // 判定是否在底座的物理射界内
            if (dist < minDistance && IsInPhysicalLimit(enemy.transform.position))
            {
                minDistance = dist;
                bestTarget = enemy.transform;
            }
        }
        currentTarget = bestTarget;
    }

    bool IsInPhysicalLimit(Vector3 targetPos)
    {
        Vector3 toEnemy = (targetPos - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toEnemy);
        return angle <= rotationLimitAngle / 2f;
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = currentTarget.position - gunNode.position;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // 使用 LerpAngle 平滑旋转并限制在 localRotation
        float currentY = gunNode.localEulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, targetAngle, rotateSpeed * Time.deltaTime);

        gunNode.rotation = Quaternion.Euler(0, newY, 0);
    }
}