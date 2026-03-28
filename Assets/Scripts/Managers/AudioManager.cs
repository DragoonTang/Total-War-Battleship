using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

/// <summary>
/// 🐉 音频管理器：负责全局音效和音乐的播放与控制
/// </summary>
public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("混音器设置")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";
    private const float minDb = -80f; // 最小分贝（静音）

    [Header("全局播放器")]
    private AudioSource musicSource; // 负责背景音乐 (2D)
    private AudioSource uiSource;    // 负责UI音效 (2D)
    // 存储当前线性音量（0-1），用于取消静音时恢复
    private float musicLinear = 1f;
    private float sfxLinear = 1f;

    [Header("音频库")]
    // 可以在 Inspector 中直接拖入常用的音频片段
    [SerializeField] private List<SoundData> soundLibrary;
    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();
        InitializeSources();
        InitializeLibrary();
    }

    private void InitializeSources()
    {
        // 创建背景音乐播放器
        musicSource = GetComponent<AudioSource>();

        // 如果你一个都没挂，它才会创建一个新的
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // 创建UI音效播放器
        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.loop = false;
        uiSource.playOnAwake = false;
        if (sfxMixerGroup != null)
            uiSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    private void InitializeLibrary()
    {
        foreach (var data in soundLibrary)
        {
            if (!string.IsNullOrEmpty(data.name) && data.clip != null)
            {
                clipDict[data.name] = data.clip;
            }
        }
    }

    // --- 公共调用接口 ---

    /// <summary>
    /// 获取音频片段
    /// </summary>
    public AudioClip GetClip(string clipName)
    {
        if (clipDict.TryGetValue(clipName, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning($"AudioManager: 找不到音效 {clipName}");
        return null;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayMusic(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    // 将 0-1 的线性音量转换为分贝并设置到混音器
    private float LinearToDb(float linear)
    {
        if (linear <= 0f) return minDb;
        return 20f * Mathf.Log10(Mathf.Clamp(linear, 0.000001f, 1f));
    }

    /// <summary>
    /// 设置背景音乐音量，传入值为 0-1（线性）。UI 传入的是 0-1。
    /// </summary>
    public void SetMusicVolumeFromLinear(float linear)
    {
        musicLinear = Mathf.Clamp01(linear);
        float db = LinearToDb(musicLinear);
        if (masterMixer != null && !string.IsNullOrEmpty(musicVolumeParam))
            masterMixer.SetFloat(musicVolumeParam, db);
        // 同时更新 AudioSource 的音量以保证非 Mixer 路由也能工作
        if (musicSource != null) musicSource.volume = musicLinear;
    }

    /// <summary>
    /// 设置音效（SFX）音量，传入值为 0-1（线性）。
    /// </summary>
    public void SetSFXVolumeFromLinear(float linear)
    {
        sfxLinear = Mathf.Clamp01(linear);
        float db = LinearToDb(sfxLinear);
        if (masterMixer != null && !string.IsNullOrEmpty(sfxVolumeParam))
            masterMixer.SetFloat(sfxVolumeParam, db);
        if (uiSource != null) uiSource.volume = sfxLinear;
    }

    public float GetMusicLinearVolume() => musicLinear;
    public float GetSFXLinearVolume() => sfxLinear;

    /// <summary>
    /// 静音/取消静音背景音乐
    /// </summary>
    public void SetMusicMute(bool mute)
    {
        if (masterMixer != null && !string.IsNullOrEmpty(musicVolumeParam))
        {
            masterMixer.SetFloat(musicVolumeParam, mute ? minDb : LinearToDb(musicLinear));
        }
        if (musicSource != null) musicSource.mute = mute;
    }

    /// <summary>
    /// 静音/取消静音音效
    /// </summary>
    public void SetSFXMute(bool mute)
    {
        if (masterMixer != null && !string.IsNullOrEmpty(sfxVolumeParam))
        {
            masterMixer.SetFloat(sfxVolumeParam, mute ? minDb : LinearToDb(sfxLinear));
        }
        if (uiSource != null) uiSource.mute = mute;
    }

    /// <summary>
    /// 播放2D音效（UI等）
    /// </summary>
    public void PlayUISound(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            uiSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 通用播放音效方法，可用于任何需要播放音效的地方（3D或2D）
    /// </summary>
    public void PlaySound(string clipName, Vector3 worldPosition, bool use3D = true)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            if (use3D)
            {
                AudioSource.PlayClipAtPoint(clip, worldPosition, sfxLinear);
            }
            else
            {
                uiSource.PlayOneShot(clip, sfxLinear);
            }
        }
    }

    /// <summary>
    /// 播放2D音效并指定音量
    /// </summary>
    public void PlaySoundWithVolume(string clipName, float volume = 1f)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            uiSource.PlayOneShot(clip, volume);
        }
    }
}

[System.Serializable]
public struct SoundData
{
    public string name;
    public AudioClip clip;
}