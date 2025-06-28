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

    public List<Transform> targets;
    public Transform muzzle; // 炮口
    public Transform gun;
    public float radius = 5f; // 扇形半径
    public float angle = 90f; // 扇形角度范围
    public float rotateSpeed = 90f; // 炮塔旋转速度（度/秒）
    public float fireThreshold = 5f; // 瞄准阈值（角度）
    public float fireRate = 3f; // 射击间隔（每秒）

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

    bool IsEnemyInSector(Vector2 enemyPos)
    {
        // 计算敌人与扇形中心的距离
        float distance = Vector2.Distance(muzzle.position, enemyPos);

        // 判断是否在半径范围内
        if (distance > radius) return false;

        // 计算敌人相对于中心的角度
        Vector2 direction = enemyPos - (Vector2)muzzle.position;
        float enemyAngle = Vector2.Angle(muzzle.up, direction);

        // 判断是否在角度范围内
        return enemyAngle <= angle / 2;
    }

    void TriggerAttack(Transform enemy)
    {
        currentTarget = enemy;
        print("开始瞄准");
        state = WeaponState.Attacking;
    }

    void AimInUpdate()
    {
        if (currentTarget == null) return; // 如果没有目标，直接返回

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
    }
}
