using PrimeTween;
using System;
using UnityEngine;

/// <summary>
/// 要受伤的物体可以绑。但是最好有碰撞
/// </summary>
public class Damageable : MonoBehaviour
{
    /// <summary>
    /// 参数为伤害量
    /// </summary>
    public event Action<int> OnHit;

    [SerializeField]
    private int currentHP;
    public int maxHP = 100;

    [SerializeField]
    internal bool isEnemy;

    [Header("特效")]
    [SerializeField] GameObject deadEffect;
    [SerializeField] Transform deadEffectPos;

    [Header("沉没动画参数")]
    [SerializeField] private float sinkDuration = 2f;  // 沉没动画时长
    [SerializeField] private float sinkDistance = 5f;  // 沉没距离
    [SerializeField] private float tiltAngle = 15f;    // 倾斜角度 (度数)

    public BoxCollider Box { get; private set; }

    void Awake()
    {
        currentHP = maxHP;
        Box = GetComponent<BoxCollider>();
        print(Box);
    }

    public void TakeDamage(int amount)
    {
        // 播放中弹音效 - 通过舰船的效果音播放器
        OnHit?.Invoke(amount);

        if (currentHP > 0)
        {
            currentHP -= amount;
            if (currentHP <= 0) Die();
        }
    }

    void Die()
    {
        // 1. 停止物理
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        BattleSceneController.Instance.UnregisterEntity(this);

        if (deadEffect != null)
        {
            GameObject effect = Instantiate(deadEffect, deadEffectPos.position, Quaternion.identity);
        }

        // 目标位置：当前局部位置 + 向下偏移
        Vector3 localEndPos = transform.localPosition + Vector3.down * sinkDistance;

        // 目标旋转：当前局部旋转 * 倾斜偏移
        Quaternion localEndRot = transform.localRotation * Quaternion.Euler(0, 0, tiltAngle);

        // 执行动画
        Tween.LocalPosition(transform, localEndPos, sinkDuration, Ease.InQuad);
        Tween.LocalRotation(transform, localEndRot, sinkDuration, Ease.InQuad);

        Tween.Delay(sinkDuration, () => gameObject.SetActive(false));
    }
}
