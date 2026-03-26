using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("战斗参数")]
    public bool isEnemy;
    public Projectile projectilePrefab;
    public float maxRange = 30f;
    public float fireRate = 2f;
    public float fireThreshold = 3f;
    public bool isAuto = true;

    [Header("特效")]
    [SerializeField] GameObject effect;

    private float cooldownTimer = 0f;
    private List<Damageable> targetList;

    public void Initialize(bool side, List<Damageable> targets)
    {
        isEnemy = side;
        targetList = targets;
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (isAuto && cooldownTimer <= 0 && targetList != null)
        {
            if (CheckAndFire())
            {
                cooldownTimer = fireRate;
            }
        }
    }

    public bool TryFire()
    {
        if (cooldownTimer > 0)
            return false;

        ExecuteLaunch();
        cooldownTimer = fireRate;
        return true;
    }

    private bool CheckAndFire()
    {
        float sqrMaxRange = maxRange * maxRange;

        foreach (var enemy in targetList)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.Box == null)
                continue;

            // 1. 使用投影矩形平方距离判定射程
            float sqrDist = CombatUtils.SqrDistanceToBox(transform.position, enemy.Box);
            if (sqrDist > sqrMaxRange)
                continue;

            // 2. 准心对齐判定：利用 CombatUtils 的角度判定工具
            if (CombatUtils.IsInAngle(transform, enemy.transform.position, fireThreshold))
            {
                ExecuteLaunch();
                return true;
            }
        }
        return false;
    }

    private void ExecuteLaunch()
    {
        effect.SetActive(true);

        GameObject bulletObj = SimplePool.Instance.Spawn(projectilePrefab.gameObject, transform.position, transform.rotation);
        if (bulletObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Initialize(isEnemy, transform.root);
        }
    }
}