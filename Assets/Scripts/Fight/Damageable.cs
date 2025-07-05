using UnityEngine;

/// <summary>
/// 要受伤的物体可以绑。但是最好有碰撞
/// </summary>
public class Damageable : MonoBehaviour
{
    [SerializeField]
    private int currentHP;

    public int maxHP = 100;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        BattleSceneController.Instance.EnemyDie(transform); // 通知战斗场景控制器敌人死亡
        // 可以调用事件或特效
        gameObject.SetActive(false);
    }
}
