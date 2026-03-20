using UnityEngine;

public class UnitCore : MonoBehaviour
{
    public bool isEnemy;

    void Start()
    {
        // 1. 明确寻找并设置身份
        // 这种方式即便子物体挂了多个脚本，也能全部覆盖
        foreach (var turret in GetComponentsInChildren<Turret>())
            turret.isEnemy = isEnemy;

        foreach (var weapon in GetComponentsInChildren<Weapon>())
            weapon.isEnemy = isEnemy;

        foreach (var health in GetComponentsInChildren<Damageable>())
            health.isEnemy = isEnemy;

        // 2. 注册到全局场景管理器 (使用根节点的 Damageable)
        if (TryGetComponent<Damageable>(out var mainHP))
        {
            BattleSceneController.Instance.RegisterEntity(mainHP);
        }
    }
}
