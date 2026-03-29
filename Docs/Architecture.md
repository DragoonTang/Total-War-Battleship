# Total War Battleship — 项目架构文档

> 生成日期：2026-03-29

---

## 一、目录结构总览

```
Assets/
├── InputSystem_Actions.cs          ← 自动生成的输入系统代码（源头文件，勿移动）
├── InputSystem_Actions.inputactions ← Input Action 配置资产
├── Scripts/
│   ├── MonoSingleton.cs            ← 泛型单例基类
│   ├── SimplePool.cs               ← 对象池（MonoSingleton）
│   ├── Common/
│   │   └── CombatUtils.cs          ← 战斗辅助静态工具
│   ├── Fight/
│   │   ├── Weapon.cs               ← 武器：冷却、自动瞄准、发射
│   │   ├── Turret.cs               ← 炮塔：旋转跟踪、射界限制
│   │   ├── Projectile.cs           ← 炮弹：运动、碰撞、伤害
│   │   ├── AttackRangeVisualizer.cs← 射程可视化（调试用）
│   │   └── BoomProjectileParticle.cs← 爆炸粒子生命周期管理
│   ├── Managers/
│   │   ├── GameManager.cs          ← 全局入口：输入初始化、镜头绑定
│   │   ├── BattleSceneController.cs← 战场管理器：单位注册、胜负判定
│   │   ├── AudioManager.cs         ← 音频管理器（MonoSingleton）
│   │   ├── UIManager.cs            ← UI 管理器（MonoSingleton）
│   │   └── CinemachineCameraControl.cs ← Cinemachine 镜头控制
│   ├── Units/
│   │   ├── UnitCore.cs             ← 舰船指挥官：音效、炮塔/武器初始化
│   │   ├── Ship.cs                 ← 舰船物理运动
│   │   └── Damageable.cs           ← 生命值、受伤、死亡动画
│   └── UI/
│       ├── GameOverPanel.cs        ← 胜利/失败面板
│       └── PauseTrigger.cs         ← 暂停系统
├── Prefabs/
│   ├── Ship/                       ← 舰船 Prefab
│   ├── Projectile.prefab
│   ├── U_Boat.prefab
│   ├── BigExplosionEffect.prefab
│   ├── DamageExplosionEffect.prefab
│   ├── MuzzleCannonEffect.prefab
│   └── PlasmaExplosionEffect.prefab
├── Scenes/
│   ├── SampleScene.unity           ← 主场景
│   └── Test.unity
├── Audio/
├── Arts/
│   ├── Materials/
│   ├── Meshes/
│   └── Textures/
├── Plugins/
│   └── PrimeTween/                 ← 动画插件（沉船动画）
└── -/                              ← 第三方资产包
    ├── EffectExamples/
    ├── Military vehicles (Sea)/
    ├── Simple Water Shader/
    └── ...
```

---

## 二、系统架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                        INPUT LAYER                              │
│  InputSystem_Actions (Assets/InputSystem_Actions.cs)           │
│  ┌─────────────────────────┬─────────────────────────────┐     │
│  │  Player ActionMap       │  UI ActionMap               │     │
│  │  Attack / Move / Look…  │  ScrollWheel                │     │
│  └────────────┬────────────┴────────────┬────────────────┘     │
└───────────────│─────────────────────────│────────────────────────┘
                │                         │
      ┌─────────▼─────────┐   ┌───────────▼──────────────┐
      │   GameManager     │   │  CinemachineCameraControl │
      │ (场景入口/镜头绑定)│   │  (双指捏合/滚轮缩放/旋转) │
      └─────────┬─────────┘   └──────────────────────────┘
                │
┌───────────────▼─────────────────────────────────────────────────┐
│                     MANAGER LAYER (单例)                        │
│  ┌────────────────────────────────────────────────────────┐    │
│  │  BattleSceneController (MonoSingleton)                 │    │
│  │  playerUnits[]  ←→  enemyUnits[]                       │    │
│  │  RegisterEntity() / UnregisterEntity() / CheckStatus() │    │
│  └────────────────────────────────────────────────────────┘    │
│  ┌──────────────────┐  ┌──────────────┐  ┌──────────────────┐  │
│  │  AudioManager    │  │  UIManager   │  │  SimplePool      │  │
│  │  (MonoSingleton) │  │  (MonoSingleton)│ │  (MonoSingleton) │  │
│  └──────────────────┘  └──────────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                │                   │
┌───────────────▼───────────────────▼─────────────────────────────┐
│                     UNIT LAYER (每艘舰船)                        │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  UnitCore (指挥官)                                        │   │
│  │  ├── Ship (物理运动: linearVelocity / angularVelocity)    │   │
│  │  ├── Damageable (HP / OnHit事件 / OnDie事件 / 沉船动画)  │   │
│  │  ├── AudioSource × 2 (水声 hurtAudio / 效果音 effectAudio)│   │
│  │  ├── Turret × N (炮塔旋转 / 射界扇形 / 目标锁定)         │   │
│  │  │   └── Weapon × M (冷却 / 自动开火 / SimplePool生成弹) │   │
│  │  └── Projectile (移动 / 碰撞 / TakeDamage / 特效)        │   │
│  └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
                │
┌───────────────▼──────────────────────────────────────────────────┐
│                     UI LAYER                                      │
│  UIManager → Ship.SetSpeed() / Ship.SetTurnDirection()           │
│  GameOverPanel (胜利/失败画面)                                    │
│  PauseTrigger (Time.timeScale 暂停)                               │
└───────────────────────────────────────────────────────────────────┘
```

---

## 三、核心数据流

### 3.1 攻击流程

```
玩家按下攻击键(鼠标右键/触摸)
  → InputSystem_Actions.Player.Attack.started
  → GameManager/CinemachineCameraControl 开启镜头旋转
  → Turret.ScanClosest() 每帧扫描最近目标
  → Turret 旋转 gunNode 朝向目标
  → Weapon.CheckAndFire() 确认目标在射程+射界内
  → Weapon.ExecuteLaunch()
      → SimplePool.Spawn(Projectile)
      → UnitCore.PlayEffectSound("Ship_Fire")
  → Projectile 向前飞行(带重力下坠)
  → Projectile.OnTriggerEnter 命中目标
      → Damageable.TakeDamage(damage)
      → Damageable.OnHit 事件 → UnitCore.HandleHit() → PlayEffectSound("Ship_Hit")
      → 如果 HP ≤ 0:
          → Damageable.Die()
          → PrimeTween 沉船动画
          → BattleSceneController.UnregisterEntity()
          → CheckBattleStatus() → 胜利/失败
```

### 3.2 速度控制流程

```
UI ToggleGroup 选中按钮(名称含数值如"0.75")
  → UIManager 解析按钮名称 → float telegraph = 0.75f
  → Ship.SetSpeed(0.75f) → targetSpeed = 0.75 * maxSpeed
  → Ship.FixedUpdate: currentSpeed MoveTowards targetSpeed
  → UnitCore.speedPercent = currentSpeed / maxSpeed
  → hurtAudio.volume / pitch 随速度变化
```

### 3.3 镜头控制流程

```
鼠标滚轮 / 双指捏合
  → InputSystem_Actions.UI.ScrollWheel / Touchscreen
  → CinemachineCameraControl.ApplyZoom(delta)
  → CinemachineOrbitalFollow.Radius = Clamp(Radius + delta, min, max)

鼠标右键按下/释放
  → InputSystem_Actions.Player.Attack started/canceled
  → CinemachineInputAxisController.enabled = true/false
```

---

## 四、关键组件说明

| 组件 | 挂载对象 | 职责 |
|------|----------|------|
| `GameManager` | 场景根节点 | 输入初始化、Cinemachine 绑定 |
| `BattleSceneController` | 场景根节点 | 单位注册表、胜负判定 |
| `AudioManager` | 场景根节点 | 全局音频混合器，clip 库 |
| `UIManager` | Canvas | UI 控件绑定、速度/转向指令分发 |
| `CinemachineCameraControl` | Virtual Camera | 镜头缩放/旋转控制 |
| `UnitCore` | 每艘舰船根 | 初始化武器/炮塔，播放音效 |
| `Ship` | 每艘舰船根 | Rigidbody 物理驱动 |
| `Damageable` | 每艘舰船根 | HP 管理、死亡事件、沉船动画 |
| `Turret` | 炮塔子节点 | 旋转跟踪、射界扇形判断 |
| `Weapon` | 炮口子节点 | 冷却计时、发射逻辑 |
| `Projectile` | 炮弹 Prefab | 飞行物理、碰撞伤害 |
| `SimplePool` | 场景根节点 | 炮弹/特效对象池 |

---

## 五、外部依赖

| 依赖 | 用途 |
|------|------|
| Unity New Input System (`com.unity.inputsystem`) | 鼠标、键盘、触摸输入 |
| Cinemachine (`com.unity.cinemachine`) | 轨道镜头、输入轴控制 |
| PrimeTween | 沉船动画（位置/旋转 Tween） |
| Military vehicles (Sea) | 舰船 3D 模型 |
| Simple Water Shader | 海洋水面着色器 |

---

## 六、当前问题诊断与解决方案

### 问题：InputSystem_Actions.cs 重复定义 (CS0101)

#### 根本原因

项目中存在**两份** `InputSystem_Actions.cs`：

```
Assets/InputSystem_Actions.cs          ← 原始自动生成文件（已提交 git）
Assets/Scripts/InputSystem_Actions.cs  ← 重复副本（git 未追踪，近期新增）
```

Unity 编译时在同一程序集中发现两个 `InputSystem_Actions` 类定义，触发：

```
CS0101: The namespace '<global namespace>' already contains a definition for 'InputSystem_Actions'
```

所有引用了 `InputSystem_Actions` 的脚本（`GameManager.cs`、`CinemachineCameraControl.cs`）都会因此无法编译，导致项目整体报错。

#### 产生原因

`InputSystem_Actions.cs` 是由 Unity 根据 `Assets/InputSystem_Actions.inputactions` **自动生成**的。
该文件原本位于 `Assets/` 根目录。某次操作将其**手动复制或移动**到了 `Assets/Scripts/` 下，导致两份文件同时存在。

#### 解决方案

**删除** `Assets/Scripts/InputSystem_Actions.cs` 及其 meta 文件：

```
Assets/Scripts/InputSystem_Actions.cs       ← 删除此文件
Assets/Scripts/InputSystem_Actions.cs.meta  ← 删除此文件
```

保留 `Assets/InputSystem_Actions.cs`（与 `.inputactions` 资产文件同目录，是 Unity 自动生成的正确位置）。

> **注意：** 不要直接修改 `Assets/InputSystem_Actions.cs`，该文件由 Unity InputSystem 包自动维护。
> 如果需要重新生成，在 Project 窗口中选中 `InputSystem_Actions.inputactions` → Inspector → **Generate C# Class**。

#### 操作步骤（在 Unity Editor 外执行）

1. 关闭 Unity Editor
2. 删除以下两个文件：
   - `Assets/Scripts/InputSystem_Actions.cs`
   - `Assets/Scripts/InputSystem_Actions.cs.meta`
3. 重新打开 Unity Editor，等待重新编译

编译完成后所有报错应全部消失。

---

### 次要问题：OnDisable 中事件取消订阅写法有误

**位置：** `GameManager.cs:47-49`

```csharp
// 错误写法：lambda 每次创建新实例，-= 无法正确取消订阅
inputActions.Player.Attack.started -= ctx => cinemaController.enabled = true;
inputActions.Player.Attack.canceled -= ctx => cinemaController.enabled = false;
inputActions.UI.ScrollWheel.performed += ctx => ...  // ← 这里写的是 += 而不是 -=
```

建议改为字段缓存 lambda，或直接调用 `inputActions.Disable()` 即可（已在第 47 行调用，功能上不影响实际运行）。

---

## 七、架构健康度评估

| 方面 | 状态 | 说明 |
|------|------|------|
| 模块解耦 | 良好 | Manager 层使用单例，Unit 层通过事件通信 |
| 对象复用 | 良好 | SimplePool 管理炮弹和特效 |
| 输入系统 | 良好 | 使用 New Input System，支持多平台 |
| 音频系统 | 良好 | AudioMixer 统一管理，支持音量调节 |
| 编译错误 | **严重** | 重复类定义导致整个项目无法编译 |
| 事件订阅 | 轻微问题 | GameManager.OnDisable 中 lambda 取消订阅写法不规范 |
