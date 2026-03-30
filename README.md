# Total War Battleship

> 一款基于 Unity 3D 的海战策略游戏，支持 PC 与移动端双平台。

---

## 项目概述

玩家指挥战舰舰队，通过控制速度与转向机动，利用自动炮塔系统消灭敌方舰队。游戏具备完整的伤害、沉船动画、胜负判定与音效反馈体系。

---

## 技术栈

| 项目 | 版本 |
|------|------|
| Unity | 6000.x (URP) |
| Input System | 1.19.0 |
| Cinemachine | 3.1.4 |
| PrimeTween | 本地插件 |
| Universal Render Pipeline | 17.3.0 |
| Post Processing | 3.5.1 |
| 目标平台 | PC / Android / iOS |

---

## 已实现功能

### 舰船控制
- 速度档位控制（0% / 25% / 50% / 75% / 100%）
- 三档转向（左舵 / 直行 / 右舵）
- 基于 Rigidbody 的物理运动，低速时转弯效率自动衰减
- 引擎音效随速度动态变化（音调 + 音量）

### 战斗系统
- 炮塔自动追踪最近目标
- 每个炮塔拥有独立射界限制（默认 ±60°）
- 距离与角度双重判定后自动开火
- 抛物线弹道（重力模拟）
- 弹丸拥有阵营属性，不会打中友军
- 每门武器独立冷却时间

### 伤害与死亡
- HP 血量系统
- 中弹触发命中音效
- 击沉时执行沉船动画（PrimeTween 下沉 + 侧倾）
- 死亡后注销至战场控制器，触发胜负检测

### 摄像机系统（Cinemachine）
- PC：右键拖拽旋转 / 滚轮缩放
- 移动端：单指旋转 / 双指捏合缩放
- 缩放范围限制（2 ~ 20 单位）

### 音频系统
- 背景音乐（循环）
- 2D UI 音效（点击、炮击、爆炸）
- 3D 空间定位音效
- AudioMixer 分组音量控制（dB 换算）
- 静音开关（含音量恢复）

### UI 系统
- 速度 / 转向按钮组（ToggleGroup）
- 音量滑条与静音开关
- 分层暂停系统（支持多个 UI 面板叠加）
- 胜负面板（含场景重载）

### 工程系统
- `MonoSingleton<T>` 泛型单例基类
- `SimplePool` 对象池（弹丸 + 粒子特效复用）
- `CombatUtils` 静态工具（近似距离计算、扇形角度判断）
- 事件驱动通信（`OnHit` / `OnDie`）

---

## 项目结构

```
Assets/
├── Scripts/
│   ├── Managers/          # 游戏管理层
│   │   ├── GameManager.cs
│   │   ├── BattleSceneController.cs
│   │   ├── AudioManager.cs
│   │   ├── UIManager.cs
│   │   └── CinemachineCameraControl.cs
│   ├── Units/             # 舰船单位层
│   │   ├── UnitCore.cs
│   │   ├── Ship.cs
│   │   └── Damageable.cs
│   ├── Fight/             # 战斗系统层
│   │   ├── Turret.cs
│   │   ├── Weapon.cs
│   │   ├── Projectile.cs
│   │   ├── AttackRangeVisualizer.cs
│   │   └── BoomProjectileParticle.cs
│   ├── UI/                # UI 层
│   │   ├── GameOverPanel.cs
│   │   └── PauseTrigger.cs
│   └── Common/            # 工具层
│       ├── MonoSingleton.cs
│       ├── SimplePool.cs
│       └── CombatUtils.cs
├── Prefabs/               # 预制体
├── Scenes/                # 场景文件
├── Audio/                 # 音频资源
├── Arts/                  # 美术资源
└── Plugins/               # 第三方插件（PrimeTween）
Docs/                      # 架构文档
```

---

## 架构分层

```
UIManager / CinemachineCameraControl
        ↓ 指令
BattleSceneController ← 注册/注销单位
        ↓
  UnitCore（每艘舰船）
  ├── Ship（物理运动）
  ├── Damageable（血量/死亡）
  └── Turret[] → Weapon[] → Projectile
                               ↓
                         TakeDamage()

AudioManager  ←  UnitCore / Weapon（音效委托）
SimplePool    ←  Weapon / BoomProjectileParticle（对象复用）
CombatUtils   ←  Turret / Weapon（距离 & 角度计算）
```

---

## 快速上手

1. 打开 `Assets/Scenes/SampleScene.unity`
2. 点击 Play
3. PC 操作：
   - 点击速度按钮控制油门
   - 点击转向按钮控制舵
   - 右键拖动旋转视角，滚轮缩放
4. 击沉全部敌舰即可胜利

---

## 已知问题

- `Assets/Scripts/InputSystem_Actions.cs` 与 `Assets/InputSystem_Actions.cs` 存在重复定义（CS0101）
  - **解决方案：** 删除 `Assets/Scripts/InputSystem_Actions.cs`，保留根目录版本

---

## 文档

- 详细架构说明：[Docs/Architecture.md](Docs/Architecture.md)
