USS MONITOR — FBX ANIMATED TEST PACKAGE
莫尼特号：带动画与贴图的 FBX 测试包

【从这里开始】
请完整解压，再导入根目录的 USS_Monitor_Animated_AllInOne.fbx。
它包含完整 LOD0 舰船模型、6 根骨骼、7 段命名动画、UV、材质和内嵌贴图。
不要直接在 ZIP 内拖拽文件测试，以免导入器无法访问旁边的贴图目录。

【内容】
USS_Monitor_Animated_AllInOne.fbx — 单文件主模型，含全部动画与内嵌贴图。
Textures/ — 22 张独立 PNG 贴图，供引擎手动关联。请保留此目录。
Animation_Clips/ — 7 个独立动画 FBX，每个只含同一骨架与单段动画。
Source/USS_Monitor_Animated_Game.blend — 原生 Blender 备份，保留完整材质和独立 NLA 预览。
Preview/Navigation_Preview.mp4 — 螺旋桨与尾舵同时运行的 8 秒预览。
Metadata/PBR_Materials.json — 材质与贴图对应关系，以及金属度等常量。
Metadata/Animation_Manifest.json — 动画、骨骼、默认舵角和转速信息。
Metadata/FBX_Package_Validation.json — 本次完整 FBX 的重新导入检查。
Animation_Blending_Guide.md — 推进与转向独立融合说明。
SHA256SUMS.txt — 包内文件完整性校验值。

【动画】
Propeller_Forward_Loop / Propeller_Reverse_Loop：螺旋桨正转与反转，1 秒一圈。
Rudder_Left / Rudder_Center / Rudder_Right：左 30 度、居中、右 30 度恒定姿态。
Rudder_Sweep_Preview：8 秒左右摆动预览。
Turret_360_Demo：10 秒炮塔旋转演示。
部分导入器会在动作名前加 Monitor_Rig 等前缀，这是正常的。

【导入要点】
- 使用通用/机械骨骼，不要套用人形骨架。
- 开启 Import Animations，并为需要循环的动作开启 Loop。
- 若导入器不支持一次读取所有动画，先导入主 FBX 的网格和骨架，再导入
  Animation_Clips 中各文件，并关联同一 Skeleton。不要重复生成整舰网格。
- 推进层只控制 Propeller_Spin；转向层只控制 Rudder_Steer。
  两层同时运行，避免整身覆盖。Root 不应启用根运动。
- 原始工作单位为米。Blender 中舰艏 +X、左舷 +Y、上方 +Z；FBX 已进行轴转换。
- 本次为 160,146 三角面、10 个主要网格、6 根骨骼的最高精度测试包。
  未在本包重复加入低精度 LOD 模型。

【贴图与材质】
FBX 已内嵌全部 22 张源贴图，同时附上外部 PNG。
FBX 对 PBR 材质的自动还原取决于导入器，必要时按 PBR_Materials.json 手动连接：
Base Color 使用 sRGB；Roughness 和 Normal 使用线性/非颜色数据。
金属度使用 JSON 中的材质常量；没有单独的 Metallic PNG。
法线为 OpenGL +Y 切线空间约定，采用 DirectX 约定的管线需转换绿色通道。
UV0 使用 Repeat 平铺，部分坐标在 0–1 范围外，没有独占光照 UV1。
原生 .blend 保留 FBX 无法完整表达的 Blender 材质与 NLA 设置。

【测试范围】
本次已检查主 FBX 重新导入后的网格、骨骼、7 段动作、UV、蒙皮，以及
22 张非空内嵌图像数据与贴图文件关联。附有原动画版本的独立/叠加播放验证。
尚未在具体游戏引擎中验证性能或控制器。船体移动、转弯与水动力由游戏逻辑驱动。
60 rpm 和左右各 30 度是可调整的游戏演示参数。

English quick start:
Extract the entire archive. Import USS_Monitor_Animated_AllInOne.fbx with mesh,
skin and animations enabled. The main file contains all seven animation takes
and embedded texture media. External textures and material mappings are also
included. If the importer cannot read all takes, import Animation_Clips/*.fbx
onto the same skeleton. Use separate bone-masked layers for Propeller_Spin and
Rudder_Steer. Disable root motion. See the Blender source for native shader/NLA
settings that FBX cannot fully represent.
