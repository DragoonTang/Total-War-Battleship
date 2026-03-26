using UnityEngine;

[RequireComponent(typeof(Damageable))]

/// <summary>
/// 指挥官
/// </summary>
public class UnitCore : MonoBehaviour
{
    public bool isEnemy;

    void Start()
    {
        // 注册到战斗控制器,确定要打的名单引用
        var targets = BattleSceneController.Instance.RegisterEntity(GetComponent<Damageable>(), isEnemy);

        GetComponent<Damageable>().isEnemy = isEnemy;

        // 统一初始化所有炮塔
        Turret[] turrets = GetComponentsInChildren<Turret>();
        foreach (var t in turrets)
        {
            t.Initialize(isEnemy, targets);
        }

        //  统一初始化所有武器
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (var w in weapons)
        {
            w.Initialize(isEnemy, targets);
        }
    }
}