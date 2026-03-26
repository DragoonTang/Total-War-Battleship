using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 🐉 战斗场景管理器：负责全局单位监控、UI指令分发及胜负判定
/// </summary>
public class BattleSceneController : MonoSingleton<BattleSceneController>
{
    [SerializeField]
    private Ship playerShip; // 玩家船只引用，用于UI控制

    // 分离双阵营列表，便于胜负判定
    public List<Damageable> playerUnits = new List<Damageable>();
    public List<Damageable> enemyUnits = new List<Damageable>();

    [Header("胜利/失败界面")]
    public GameOverPanel overPanel;

    private void Start()
    {
        // 确保面板初始关闭
        if (overPanel) overPanel.gameObject.SetActive(false);

        // UI 事件绑定
        //btnSpeed1.onClick.AddListener(() => playerShip.SetSpeed(0.25f));
        //btnSpeed2.onClick.AddListener(() => playerShip.SetSpeed(0.5f));
        //btnSpeed3.onClick.AddListener(() => playerShip.SetSpeed(0.75f));
        //btnSpeed4.onClick.AddListener(() => playerShip.SetSpeed(1f));
        //btnStop.onClick.AddListener(() => playerShip.SetSpeed(0f));
        //btnReverse.onClick.AddListener(() => playerShip.SetSpeed(-0.5f));

        //btnTurnLeft.onClick.AddListener(() => playerShip.SetTurnDirection(-1));
        //btnTurnStop.onClick.AddListener(() => playerShip.SetTurnDirection(0));
        //btnTurnRight.onClick.AddListener(() => playerShip.SetTurnDirection(1));


    }

    #region 自动化注册系统
    /// <summary>
    /// 由 Damageable 或 UnitCore 在 Start 时调用
    /// </summary>
    public List<Damageable> RegisterEntity(Damageable unit, bool isEnemy)
    {
        if (isEnemy)
        {
            enemyUnits.Add(unit);
            return playerUnits; // 返回玩家单位列表，供敌人AI锁定
        }
        else
        {
            playerUnits.Add(unit);
            return enemyUnits; // 返回敌人单位列表，供玩家锁定
        }
    }

    /// <summary>
    /// 由 Damageable 在死亡时调用
    /// </summary>
    public void UnregisterEntity(Damageable unit)
    {
        if (unit.isEnemy)
        {
            enemyUnits.Remove(unit);
        }
        else
        {
            playerUnits.Remove(unit);
        }

        CheckBattleStatus();
    }
    #endregion

    /// <summary>
    /// 核心胜负判定逻辑
    /// </summary>
    private void CheckBattleStatus()
    {
        // 敌人清空，玩家还在 -> 胜利
        if (enemyUnits.Count == 0 && playerUnits.Count > 0)
        {
            EndGame(true);
        }
        // 玩家单位全部清空 -> 失败
        else if (playerUnits.Count == 0)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool isVictory)
    {
        Debug.Log(isVictory ? "战斗胜利！" : "战斗失败！");

        overPanel.gameObject.SetActive(true);
        overPanel.Show(isVictory);
    }

}