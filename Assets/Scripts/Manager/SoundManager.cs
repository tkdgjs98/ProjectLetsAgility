using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
            }

            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void InitStatic()
    {
        _instance = null;
    }
    
    private AudioSource _bgmSource;
    private List<AudioSource> _audioSourceList;
    private int _currentSfxSourceIndex = 0;

    [SerializeField] private List<AudioClip> bgmClipList;
    [SerializeField] private List<AudioClip> sfxClipList;

    private Dictionary<string, AudioClip> _audioClipBgmDict;
    private Dictionary<string, AudioClip> _audioClipSfxDict;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Init();
        DontDestroyOnLoad(Instance.gameObject);
    }
    
    private void Init()
    {
        Debug.Log("Init SoundManager");
        _audioSourceList = new List<AudioSource>();
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.volume = 0.5f;
        // TODO FIX BGM VOLUME
        for (int i = 0; i < 15; i++)
        {
            _audioSourceList.Add(gameObject.AddComponent<AudioSource>());
        }

        _audioClipBgmDict = new Dictionary<string, AudioClip>();
        foreach (var audioClip in bgmClipList)
        {
            _audioClipBgmDict.Add(audioClip.name, audioClip);
        }

        _audioClipSfxDict = new Dictionary<string, AudioClip>();
        foreach (var audioClip in sfxClipList)
        {
            _audioClipSfxDict.Add(audioClip.name, audioClip);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public static void PlaySfx(string key)
    {
        Instance.PlaySfxInstance(key);
    }

    private void PlaySfxInstance(string key)
    {
        _currentSfxSourceIndex %= _audioSourceList.Count;
        var audioSource = _audioSourceList[_currentSfxSourceIndex];
        audioSource.clip = _audioClipSfxDict[key];
        audioSource.loop = false;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
        _currentSfxSourceIndex++;
    }

    public static void PlayBgm(string key)
    {
        Instance.PlayBgmInstance(key);
    }

    private void PlayBgmInstance(string key)
    {
        var clip = _audioClipBgmDict[key];
        if (clip == _bgmSource.clip)
        {
            return;
        }
        StopBgm();
        _bgmSource.clip = _audioClipBgmDict[key];
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public static void StopBgm()
    {
        Instance.StopBgmInstance();
    }

    private void StopBgmInstance()
    {
        _bgmSource.Stop();
    }
}
