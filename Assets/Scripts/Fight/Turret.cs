using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
    [Header("核心引用")]
    public bool isEnemy;
    public Transform gunNode; // 真正旋转的炮管/炮塔部分

    [Header("运动参数")]
    public float rotateSpeed = 45f;
    public float maxRange = 30f;

    [Tooltip("总射界角度。如120表示从中线向左和向右各60度")]
    public float rotationLimitAngle = 120f;

    [SerializeField]
    private List<Damageable> targetList;

    public void Initialize(bool side, List<Damageable> targets)
    {
        isEnemy = side;
        targetList = targets;

        var weapons = GetComponentsInChildren<Weapon>();
        foreach (var w in weapons)
        {
            w.maxRange = maxRange;
        }
    }

    void Update()
    {
        if (targetList == null || gunNode == null) return;

        // 寻找最近目标
        Damageable target = ScanClosest();

        // 判定逻辑：必须有目标，且目标中心点在扇形射界内（射界通常针对旋转限制，仍以中心点为参考）
        if (target != null && IsTargetInSector(target.transform.position))
        {
            RotateTowards(target.transform.position);
        }
        else
        {
            ReturnToCenter();
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 localTargetPos = transform.InverseTransformPoint(targetPos);
        localTargetPos.y = 0;

        if (localTargetPos != Vector3.zero)
        {
            Quaternion targetLocalRot = Quaternion.LookRotation(localTargetPos);
            gunNode.localRotation = Quaternion.RotateTowards(
                gunNode.localRotation,
                targetLocalRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    private void ReturnToCenter()
    {
        if (gunNode.localRotation == Quaternion.identity) return;

        gunNode.localRotation = Quaternion.RotateTowards(
            gunNode.localRotation,
            Quaternion.identity,
            rotateSpeed * Time.deltaTime
        );
    }

    private bool IsTargetInSector(Vector3 worldPos)
    {
        return CombatUtils.IsInAngle(transform, worldPos, rotationLimitAngle * 0.5f);
    }

    private Damageable ScanClosest()
    {
        float minSqrD = maxRange * maxRange;
        Damageable best = null;

        foreach (var e in targetList)
        {
            if (e == null || !e.gameObject.activeInHierarchy || e.Box == null) continue;

            // 使用 CombatUtils 计算点到 Box 投影矩形的平方距离
            float sqrD = CombatUtils.SqrDistanceToBox(transform.position, e.Box);

            if (sqrD < minSqrD)
            {
                minSqrD = sqrD;
                best = e;
            }
        }
        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -rotationLimitAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, rotationLimitAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * maxRange);
        Gizmos.DrawRay(transform.position, rightBoundary * maxRange);
    }
}