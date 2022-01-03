using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    #region 인스턴스화
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }

    public static SoundManager m_Instance;
    public static SoundManager Instance
    {
        get
        {
            if (!m_Instance)
            {
                _container = new GameObject();
                _container.name = "SoundManager";
                m_Instance = _container.AddComponent(typeof(SoundManager)) as SoundManager;
                DontDestroyOnLoad(_container);
            }

            return m_Instance;
        }
    }
    #endregion

    public AudioMixer masterMixer;

    public Slider bgmSlider;
    public Slider sfxSlider;

    public AudioSource bgmPlayer = null;
    public AudioSource[] sfxPlayer = null;

    [SerializeField] Sound[] bgm = null;
    [SerializeField] Sound[] sfx = null;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        bgmSlider.value = DataManager.Instance.gameData.bgm;
        sfxSlider.value = DataManager.Instance.gameData.sfx;

        masterMixer.SetFloat("BGM", bgmSlider.value);
    }

    private void Update()
    {
        BGMSave();
        SFXSave();
    }

    public void BGMSave()
    {
        bgmPlayer.volume = bgmSlider.value;
        DataManager.Instance.gameData.bgm = bgmSlider.value;
    }

    public void SFXSave()
    {
        for(int i = 0; i < sfxPlayer.Length; i++)
        {
            sfxPlayer[i].volume = sfxSlider.value;
        }
        
        DataManager.Instance.gameData.sfx = sfxSlider.value;
    }

    public void PlayBGM(string p_bgmName)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName == sfx[i].name)
            {
                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        sfxPlayer[x].clip = sfx[i].clip;
                        sfxPlayer[x].Play();
                        return;
                    }
                }
                return;
            }
        }
    }
}