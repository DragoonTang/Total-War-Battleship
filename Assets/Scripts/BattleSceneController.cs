using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 🐉战斗场景的管理器
/// </summary>
public class BattleSceneController : MonoSingleton<BattleSceneController>
{
    [SerializeField]
    Ship playerShip; // 玩家船只

    public List<Transform> Enemies;

    #region Button
    public Button btnSpeed1, btnSpeed2, btnSpeed3, btnSpeed4;
    public Button btnStop;
    public Button btnReverse;
    public Button btnTurnLeft, btnTurnStop, btnTurnRight;
    #endregion

    private void Start()
    {
        btnSpeed1.onClick.AddListener(() => playerShip.SetSpeed(0.25f));
        btnSpeed2.onClick.AddListener(() => playerShip.SetSpeed(0.5f));
        btnSpeed3.onClick.AddListener(() => playerShip.SetSpeed(0.75f));
        btnSpeed4.onClick.AddListener(() => playerShip.SetSpeed(1));
        btnStop.onClick.AddListener(() => playerShip.SetSpeed(0)); // 停止移动

        btnReverse.onClick.AddListener(() => playerShip.SetSpeed(-0.5f));

        btnTurnLeft.onClick.AddListener(() => playerShip.SetTurnDirection(-1));
        btnTurnStop.onClick.AddListener(() => playerShip.SetTurnDirection(0));
        btnTurnRight.onClick.AddListener(() => playerShip.SetTurnDirection(1));
    }

    public void EnemyDie(Transform enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            Debug.Log($"{enemy.name} has been defeated. Remaining enemies: {Enemies.Count}");
        }
        else
        {
            Debug.LogWarning($"{enemy.name} is not in the enemy list.");
        }
    }
}
