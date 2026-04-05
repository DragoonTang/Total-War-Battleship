using PrimeTween;
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
    public List<Damageable> playerUnits = new();
    public List<Damageable> enemyUnits = new();

    [Header("胜利/失败界面")]
    public GameOverPanel overPanel;

    [Header("结算配置")]
    [SerializeField] private float endDelay = 2.0f; // 统一延迟时间
    private Tween _endGameTween; // 用于追踪和防止重复触发的句柄

    private void Start()
    {
        // 确保面板初始关闭
        if (overPanel) overPanel.gameObject.SetActive(false);
    }

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
    public void UnregisterEntity(Damageable unit)
    {
        if (unit.isEnemy)
        {
            enemyUnits.Remove(unit);
        }
        else
        {
            playerUnits.Remove(unit);
            // 失败判定：只要玩家单位列表空了，立即执行
            if (playerUnits.Count == 0)
            {
                playerShip.enabled = false;
                CheckBattleStatus(false);
                return;
            }
        }

        // 胜利判定：敌军空了
        if (enemyUnits.Count == 0)
        {
            CheckBattleStatus(true);
        }
    }

    private void CheckBattleStatus(bool isVictory)
    {
        // 防止多个单位同时死亡导致重复开启多个延迟任务
        if (_endGameTween.isAlive) return;

        // 使用 PrimeTween 实现延迟回调
        _endGameTween = Tween.Delay(endDelay, () => EndGame(isVictory));
    }

    private void EndGame(bool isVictory)
    {
        _endGameTween.Stop(); // 安全清理
        Debug.Log(isVictory ? "战斗胜利！" : "战斗失败！");
        overPanel.gameObject.SetActive(true);
        overPanel.Show(isVictory);
    }
}