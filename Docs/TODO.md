# 《全面战舰》开发 TODO 清单

> 按**从易到难**排列。策划来源：《全面战舰》策划案 + FEATURE_ROADMAP.md 建议。
> 标注：🔴 P0 必做 / 🟡 P1 核心 / 🟢 P2 打磨 / 🔵 拓展

---

## ✅ 已完成

- [x] MonoSingleton 泛型单例基类
- [x] SimplePool 对象池（弹丸 + 粒子）
- [x] BattleSceneController 单位注册/注销与胜负判定
- [x] Ship.cs 物理运动（速度/转向/惯性）
- [x] Damageable.cs HP 系统 + PrimeTween 沉船动画
- [x] Turret.cs 炮塔自动旋转追踪（扇形射界限制）
- [x] Weapon.cs 自动开火（距离 + 角度双重判断 + 冷却）
- [x] Projectile.cs 抛物线弹道 + 阵营伤害判断
- [x] CombatUtils.cs 静态工具（SqrDistanceToBox / IsInAngle）
- [x] AudioManager 双通道音效 + AudioMixer 音量控制
- [x] UIManager 速度 / 转向 ToggleGroup 控制
- [x] CinemachineCameraControl PC + 移动端双平台摄像机
- [x] GameOverPanel 胜负弹窗 + 重启
- [x] PauseTrigger 分层暂停系统
- [x] AttackRangeVisualizer 调试可视化

---

## 🔴 LEVEL 1 — 修复 & 基础补全（30 分钟以内）

### 1. 修复编译错误（必须最先做）
- [ ] 删除重复文件 `Assets/Scripts/InputSystem_Actions.cs`
- [ ] 保留 `Assets/InputSystem_Actions.cs`（根目录版本）
- [ ] 验证项目可以正常编译运行

### 2. 速度档位扩展至 5 档
**策划要求：** 负档（倒车）、0（停车）、1-4 档提速
**当前状态：** 只有 0.25 / 0.5 / 0.75 / 1 四个正向档位
- [ ] `Ship.cs`：支持负速度（倒车，maxSpeed 的 -0.3 倍）
- [ ] `UIManager.cs`：添加倒车档按钮解析（值为负数，如 `-0.3`）
- [ ] UI 场景：在速度按钮组中添加倒车档按钮
- [ ] 验证倒车时转向仍然有效（方向反转）

### 3. 修复 GameManager.OnDisable 事件反注册
**当前状态：** Lambda 无法正确反注册，存在潜在内存泄漏
- [ ] 将 Lambda 缓存为字段变量，确保 OnDisable 中反注册成功

---

## 🔴 LEVEL 2 — UI 反馈（1-2 小时）

### 4. WorldSpace 血条（目标锁定反馈）
**策划要求：** 目标上方显示极简红色血条
- [ ] 新建 `HealthBar.cs` 脚本
- [ ] 每艘舰船挂载 WorldSpace Canvas + Slider（红色填充）
- [ ] `Damageable.OnHit` 事件触发 `Slider.value` 更新
- [ ] Canvas 始终面朝主摄像机（Billboard 旋转）
- [ ] 仅在被攻击后显示 3 秒，平时隐藏

### 5. 伤害飘字
**策划来源：** FEATURE_ROADMAP 建议
- [ ] 新建 `DamagePopup.cs`，使用 TextMeshPro
- [ ] `Damageable.TakeDamage()` 触发：命中位置生成飘字
- [ ] PrimeTween 驱动：向上飘动 + 渐隐（0.8 秒）+ 回收至对象池
- [ ] 颜色：普通伤害白色 / 高额伤害红色

---

## 🟡 LEVEL 3 — 视觉损伤系统（2-3 小时）

### 6. 分阶段视觉损伤
**策划要求：**
- HP 剩余 70% 以下：少量黑烟
- HP 剩余 30% 以下：大面积浓烟 + 火光
- [ ] `Damageable.cs` 中添加 `OnDamageStageChanged` 事件（阶段 0/1/2）
- [ ] 新建 `VisualDamageController.cs`：
  - 挂载黑烟粒子 GameObject（默认关闭）
  - 挂载火光粒子 GameObject（默认关闭）
  - 订阅阶段事件，按阈值开启/关闭粒子
- [ ] 为玩家舰船和敌方舰船各配置损伤粒子（旗舰需要"巨型身躯冒烟"效果）

### 7. 击沉动画增强
**策划要求：** 沉没 + 小幅倾斜（已有基础，需增强）
- [ ] 沉没前：触发浓烟粒子爆发（0.5 秒）
- [ ] 增加翻转序列：侧倾超过 90° 后倒扣入水
- [ ] 沉没时水面生成溅射水花特效
- [ ] 摄像机轻微震动（Camera Shake，使用 Cinemachine Impulse）

---

## 🟡 LEVEL 4 — 固定侧舷炮（2-3 小时）

### 8. 侧舷固定火炮（Broadside Weapon）
**策划要求：** 风帆护卫舰 / 旗舰的固定侧舷炮，两侧各一排，只朝侧面射击
**当前状态：** 只有旋转炮塔系统，无固定侧向炮
- [ ] 新建 `BroadsideWeapon.cs`（继承或复用 Weapon.cs 逻辑）：
  - 射界固定为船体左侧或右侧（±90° ± 30° 扇形）
  - 不需要炮塔旋转，直接判断目标是否在侧面扇区
  - 齐射模式：左侧或右侧所有炮同时触发（延迟 0.05s 间隔）
- [ ] 新建 `BroadsideGunController.cs`：
  - 管理两侧炮列（Left / Right 各一组）
  - 自动判断哪一侧更靠近目标，激活对应侧
- [ ] 视觉：开火时产生大量黑烟（体现旧式风帆战舰特色）

---

## 🟡 LEVEL 5 — 敌方 AI 移动（Dolly Track）（3-4 小时）

### 9. Cinemachine Dolly Track 敌方 AI
**策划要求：** 敌舰沿预设航线巡航
- [ ] 在 Unity 场景中创建 3 条 Cinemachine Dolly Track：
  - **航线 A/B**：护卫舰在玩家外围绕圈轨道（椭圆形）
  - **航线 C**：旗舰从远端缓慢直线逼近玩家
- [ ] 新建 `DollyTrackMover.cs`：
  - 让船体沿 CinemachinePathBase 路径移动（而非摄像机）
  - 速度参数化，支持不同舰船速度
  - 船头始终朝向路径切线方向（Quaternion.LookRotation）
- [ ] 护卫舰：循环路径，高速（优先靠近玩家侧面）
- [ ] 旗舰：非循环路径，3 分钟后开始移动（使用 `Invoke` 或协程延迟）

### 10. 旗舰定时行为
**策划要求：** 旗舰在 3 分钟后切入近场，将侧舷对准玩家
- [ ] `BattleSceneController.cs` 中添加战场计时器
- [ ] 3 分钟时广播事件 `OnFlagshipActivate`
- [ ] 旗舰 AI 收到事件后激活 Dolly Track 移动 + 尝试将侧舷朝向玩家

---

## 🟡 LEVEL 6 — 完整舰船 Prefab 配置（2-3 小时）

### 11. 玩家现代巡洋舰
- [ ] 配置 1-2 组 360° 旋转炮塔 Prefab（使用现有 Turret + Weapon）
- [ ] 填入数值：HP、速度、炮塔射程、冷却时间
- [ ] 绑定 UnitCore（注册为玩家阵营）
- [ ] 绑定 VisualDamageController

### 12. 敌方风帆护卫舰
- [ ] 配置侧舷固定炮（BroadsideWeapon，两侧各 3-4 门）
- [ ] 填入数值：HP 较低、移动速度快、炮伤害中等
- [ ] 绑定 UnitCore（注册为敌方阵营）
- [ ] 绑定 DollyTrackMover（航线 A 或 B）
- [ ] 视觉：开火时大量黑烟特效

### 13. 敌方风帆旗舰
- [ ] 配置密集侧舷火炮（两侧各 6-8 门，高威力）
- [ ] 填入数值：HP 极高（Boss 级）、速度慢
- [ ] 绑定 UnitCore（注册为敌方阵营）
- [ ] 绑定 DollyTrackMover（航线 C，3 分钟延迟激活）
- [ ] 视觉：受伤严重时冒浓烟（VisualDamageController）

---

## 🟡 LEVEL 7 — 关卡完整流程（2-3 小时）

### 14. 场景边界（空气墙）
**策划要求：** 1000x1000 海域，四周不可越界
- [ ] 在场景四周放置透明 BoxCollider（Is Trigger = false）
- [ ] 或新建 `BoundarySystem.cs`：检测越界后反弹速度向量
- [ ] 可选：边界附近显示迷雾 / 浅滩视觉提示

### 15. 完整关卡场景搭建
- [ ] 放置玩家舰船 Prefab（场景中心起始位置）
- [ ] 放置 2 艘护卫舰 Prefab + 1 艘旗舰 Prefab
- [ ] 配置 3 条 Dolly Track 路径
- [ ] 战场计时器 UI（可选：右上角显示已战斗时间）
- [ ] 验证胜利条件（敌舰全灭）和失败条件（玩家 HP 归零）

---

## 🟢 LEVEL 8 — 体验打磨（1-2 小时）

### 16. 音效补全
- [ ] 添加风帆炮侧舷齐射音效（厚重低频炮声，区别于现代炮）
- [ ] 添加木材中弹音效（命中护卫舰/旗舰时）
- [ ] 添加旗舰警报音效（3 分钟入场时）
- [ ] 背景音乐（海浪 + 战场环境音）

### 17. 开场 / 过场演出
- [ ] 战斗开始时摄像机俯冲入场动画（Cinemachine Timeline）
- [ ] 旗舰 3 分钟入场时短暂特写镜头（增强紧张感）

### 18. 暂停菜单完善
- [ ] 暂停时显示简单选项：继续 / 重新开始 / 退出
- [ ] 当前只有 PauseTrigger，需要完整 UI

---

## 🔵 LEVEL 9 — 扩展功能（长期）

### 19. 多关卡系统
- [ ] 新建 `LevelConfig` ScriptableObject（定义敌舰数量、航线、难度）
- [ ] 关卡选择 UI 场景
- [ ] 通关后显示星级评分（时间 + 受伤量）

### 20. 舰船升级系统
- [ ] 战斗结束获得经验
- [ ] 解锁更强炮塔、护甲、引擎

### 21. 多舰队指挥
- [ ] 底部舰船选择面板
- [ ] 切换激活舰船，摄像机跟随

### 22. 天气系统
- [ ] 随机天气：晴天 / 风暴
- [ ] 风暴：降低炮弹精度 + 视觉效果

---

## 优先级总览

| 级别 | 内容 | 预计时长 | 关联策划优先级 |
|------|------|----------|---------------|
| LEVEL 1 | 修复编译 + 5 档速度 | ~1h | P0 |
| LEVEL 2 | 血条 + 飘字 | ~2h | P0 |
| LEVEL 3 | 视觉损伤 + 沉船增强 | ~3h | P2 |
| LEVEL 4 | 固定侧舷炮 | ~3h | P1 |
| LEVEL 5 | Dolly Track AI + 旗舰计时 | ~4h | P1 |
| LEVEL 6 | 三艘舰船 Prefab 配置 | ~3h | P1 |
| LEVEL 7 | 场景搭建 + 边界 | ~3h | P1 |
| LEVEL 8 | 音效 + 演出打磨 | ~2h | P2 |
| LEVEL 9 | 扩展功能 | 不限 | — |
| **合计** | **完整 Demo** | **~21h** | |

---

> **建议起步顺序：**
> `LEVEL 1（修复）` → `LEVEL 2（血条）` → `LEVEL 4（侧舷炮）` → `LEVEL 5（AI 移动）` → `LEVEL 6（Prefab）` → `LEVEL 7（关卡）` → `LEVEL 3（视觉打磨）`
> 这条路径每完成一步都能在编辑器里看到明显变化，调试也更顺畅。
