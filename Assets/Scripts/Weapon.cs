using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 🐉
/// </summary>
public class Weapon : MonoBehaviour
{
    enum WeaponState
    {
        Idle, // 空闲状态
        Attacking, // 攻击状态
        Cooldown // 冷却状态
    }

    public Projectile projectilePrefab; // 子弹预制体
    public List<Transform> targets;
    /// <summary>
    /// 攻击范围（扇形）的中心点
    /// </summary>
    public Transform center;
    public Transform gun;
    public float radius = 5f; // 扇形半径
    public float angle = 90f; // 扇形角度范围
    public float rotateSpeed = 90f; // 炮塔旋转速度（度/秒）
    public float fireThreshold = 5f; // 瞄准阈值（角度）
    public float fireRate = 3f; // 射击间隔（每秒）
    public Transform firePoint; // 发射点

    Transform currentTarget;
    [SerializeField]
    WeaponState state = WeaponState.Idle;
    private bool HasFired
    {
        set
        {
            // 如果设置为 true，重置冷却时间,武器进入冷却状态
            if (value)
                cdTime = fireRate;
        }
        get
        {
            return cdTime > 0;
        }
    }

    float cdTime = 0.5f; // 攻击冷却时间

    private void FixedUpdate()
    {
        // 每次在 FixedUpdate 访问时检查冷却时间
        if (cdTime > 0)
        {
            cdTime -= Time.fixedDeltaTime;
        }
        else if (cdTime < 0)
        {
            cdTime = 0; // 确保冷却时间不会小于零
        }

        switch (state)
        {
            case WeaponState.Idle:
                foreach (var enemy in targets)
                {
                    if (IsEnemyInSector(enemy.position))
                    {
                        TriggerAttack(enemy); // 执行攻击逻辑
                    }
                }
                break;
            case WeaponState.Attacking:
                AimInUpdate();
                break;
            case WeaponState.Cooldown:
                break;
            default:
                break;
        }

    }

    bool IsEnemyInSector(Vector3 enemyPos)
    {
        Vector3 toEnemy = enemyPos - center.position;

        // 把Y分量设为0，确保只在XZ平面判断
        toEnemy.y = 0;

        if (toEnemy.magnitude > radius)
            return false;

        float angleToEnemy = Vector3.Angle(center.forward, toEnemy.normalized);

        return angleToEnemy <= angle / 2f;
    }


    void TriggerAttack(Transform enemy)
    {
        currentTarget = enemy;
        print("开始瞄准");
        state = WeaponState.Attacking;
    }

    void AimInUpdate()
    {
        // 如果没有目标或者目标不在扇形范围内，切换到空闲状态
        if (currentTarget == null||!IsEnemyInSector(currentTarget.position))
        {
            currentTarget = null; // 清除当前目标
            state = WeaponState.Idle;
            return; 
        }

        // 计算目标方向
        Vector3 direction = currentTarget.position - gun.position;

        // 计算目标角度（相对于炮塔的当前角度）
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; // 使用XZ平面计算yaw角

        // 获取当前炮塔的角度
        float currentAngle = gun.eulerAngles.y;

        // 平滑旋转到目标角度
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        // 应用旋转，仅调整Y轴
        gun.rotation = Quaternion.Euler(0, newAngle, 0);

        // 检测是否瞄准
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        if (angleDifference <= fireThreshold && !HasFired)
        {
            Fire(); // 触发攻击
            HasFired = true; // 防止重复攻击
        }
    }

    private void Fire()
    {
        print("Fire!"); // 这里可以替换为实际的攻击逻辑，比如发射子弹或播放动画
        SimplePool.Spawn(projectilePrefab.gameObject, firePoint.position, firePoint.rotation);
    }
}
