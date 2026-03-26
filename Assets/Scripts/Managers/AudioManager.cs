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

    [Header("全局播放器")]
    private AudioSource musicSource; // 负责背景音乐 (2D)
    private AudioSource uiSource;    // 负责UI音效 (2D)

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
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // 创建UI音效播放器
        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.loop = false;
        uiSource.playOnAwake = false;

        // 稍后在 Mixer 中分配 Output Group
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
}

[System.Serializable]
public struct SoundData
{
    public string name;
    public AudioClip clip;
}