using UnityEngine;


/// <summary>
/// 指挥官：负责声音、特效与战斗逻辑的统一调度
/// </summary>
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(AudioSource))]
public class UnitCore : MonoBehaviour
{
    public bool isEnemy;

    [Header("声音配置")]
    public float basePitch = 0.8f;
    public float speedPercent;
    AudioSource moveAudio;
    AudioSource effectAudio;

    [Header("受损特效配置")]
    [SerializeField] private GameObject smokeEffect;
    [SerializeField,Tooltip("冒烟阈值：生命值百分比小于该值时显示冒烟")]
    float smokeThreshold = 0.6f; 
    [SerializeField] private GameObject fireEffect;
    [SerializeField, Tooltip("燃火阈值生命值百分比")]
    float fireThreshold = 0.35f; 

    Damageable damageable;
    Weapon[] weapons;

    void Start()
    {
        // --- 原有音频初始化 ---
        moveAudio = moveAudio != null ? moveAudio : GetComponent<AudioSource>();
        if (moveAudio != null && moveAudio.clip != null)
        {
            moveAudio.Play();
        }

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 1)
        {
            effectAudio = audioSources[1];
        }

        // --- 注册 Damageable 事件 ---
        damageable = GetComponent<Damageable>();
        damageable.isEnemy = isEnemy;

        // 监听受伤：更新特效
        damageable.OnHit += (_) =>
        {
            HandleHit();
            UpdateDamageEffects(); // 每次挨打检查一次生命值
        };

        // 监听死亡：清理特效
        damageable.OnDie += () =>
        {
            HandleDeathSound();
            ClearEffectsOnDeath();
        };

        // 初始化特效状态（防止出生就是残血）
        UpdateDamageEffects();

        // 注册到战斗控制器并初始化武器
        var targets = BattleSceneController.Instance.RegisterEntity(damageable, isEnemy);

        Turret[] turrets = GetComponentsInChildren<Turret>();
        foreach (var t in turrets) t.Initialize(isEnemy, targets);

        weapons = GetComponentsInChildren<Weapon>();
        foreach (var w in weapons) w.Initialize(isEnemy, targets);
    }

    void Update()
    {
        // 保持原有的螺旋桨水声逻辑
        if (moveAudio != null)
        {
            float clampSpeed = Mathf.Clamp01(speedPercent);
            moveAudio.volume = Mathf.Lerp(0.1f, 1.0f, clampSpeed);
            moveAudio.pitch = Mathf.Lerp(basePitch, 1.25f, clampSpeed);
        }
    }

    /// <summary>
    /// 根据血量百分比控制特效开关
    /// </summary>
    private void UpdateDamageEffects()
    {
        if (damageable == null) return;

        // 小于 70% 冒烟
        if (smokeEffect != null)
        {
            smokeEffect.SetActive(damageable.Percent <= smokeThreshold);
        }

        // 小于 50% 燃火
        if (fireEffect != null)
        {
            fireEffect.SetActive(damageable.Percent <= fireThreshold);
        }
    }

    /// <summary>
    /// 死亡时关闭所有特效，防止沉船后水面还有火
    /// </summary>
    private void ClearEffectsOnDeath()
    {
        if (smokeEffect != null) smokeEffect.SetActive(false);
        if (fireEffect != null) fireEffect.SetActive(false);

        // 禁用武器组件，防止单位还在攻击
        foreach (var w in weapons)
        {
            w.enabled = false;
        }
    }

    // --- 原有音效方法 ---
    public void PlayEffectSound(string clipName)
    {
        AudioClip clip = AudioManager.Instance.GetClip(clipName);
        if (clip != null && effectAudio != null)
        {
            effectAudio.PlayOneShot(clip);
        }
        else if (clip != null)
        {
            AudioManager.Instance.PlayUISound(clipName);
        }
    }

    void HandleHit() => PlayEffectSound("Ship_Hit");
    void HandleDeathSound() => PlayEffectSound("Ship_Boom");
}