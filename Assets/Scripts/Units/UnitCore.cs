using UnityEngine;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(AudioSource))]

/// <summary>
/// 指挥官
/// </summary>
public class UnitCore : MonoBehaviour
{
    public bool isEnemy;

    [Header("声音配置")]
    [SerializeField] AudioSource myAudio;
    public float basePitch = 0.8f;
    public float speedPercent;

    // 第二个音效播放器，用于播放开火和中弹音效
    [SerializeField] private AudioSource effectAudio;

    void Start()
    {
        myAudio = myAudio != null ? myAudio : GetComponent<AudioSource>();
        if (myAudio != null && myAudio.clip != null)
        {
            myAudio.Play(); // 启动你在 Inspector 里拖好的水声
        }

        // 获取第二个音效播放器
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 1)
        {
            effectAudio = audioSources[1];
        }

        // 注册到战斗控制器
        Damageable damageable = GetComponent<Damageable>();
        damageable.OnHit += (amount) => PlayEffectSound("Ship_Hit");
        damageable.isEnemy = isEnemy;
        var targets = BattleSceneController.Instance.RegisterEntity(damageable, isEnemy);

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

    void Update()
    {
        // 只有在获取了第一个播放器（水声）的情况下执行
        if (myAudio != null)
        {
            // 限制在 0-1 之间增加安全性
            float clampSpeed = Mathf.Clamp01(speedPercent);

            // 映射音量：静止时 0.1（保底水声），全速时 1.0
            myAudio.volume = Mathf.Lerp(0.1f, 1.0f, clampSpeed);

            // 映射音调：这个是“速度感”的关键
            // 建议范围大一点，比如从 0.75f 到 1.25f
            myAudio.pitch = Mathf.Lerp(basePitch, 1.25f, clampSpeed);
        }
    }

    /// <summary>
    /// 播放音效（优先使用第二个播放器 effectAudio）
    /// </summary>
    public void PlayEffectSound(string clipName)
    {
        AudioClip clip = AudioManager.Instance.GetClip(clipName);
        if (clip != null && effectAudio != null)
        {
            effectAudio.PlayOneShot(clip);
        }
        else if (clip != null)
        {
            // 备用：使用 AudioManager 的通用播放方法
            AudioManager.Instance.PlayUISound(clipName);
        }
    }
}