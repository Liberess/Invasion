using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
//카르마 : Karma, Karma Dungeon, 캐릭터 레벨업
//각성석 : Awakening Stone, Awakening Dungeon, 캐릭터 각성 (각성 10회 -> 초월 1회 가능)
//파편 : Piece, Piece Dungeon -> 각각의 카드 해금
//소울젬 : Soul Gem, Soul Gem Dungeon -> 각각의 영웅 해금, 가테 마일리지
//진화석 : Evolution Stone, Evolution Dungeon, 캐릭터 진화, 진화마다 외형 변경

public enum DungeonType
{
    Karma,
    Evolution,
    Piece,
    SoulGem,
    Awakening
}

public class DungeonPanel : MonoBehaviour
{
    public static DungeonPanel Instance;

    public GameObject MyPanel;
    public GameObject OptionPanel;

    public DungeonType dungeonType;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        OptionFunc();
    }

    public void OptionFunc()
    {
        if (OptionPanel.activeSelf)
        {
            Invoke("TimeOff", 0.3f);

            GameManager.Instance.isPlay = false;
        }
        else
        {
            GameManager.Instance.isPlay = true;

            Time.timeScale = 1;
        }
    }

    private void TimeOff()
    {
        Time.timeScale = 0;
    }

    private void ActiveCtrl()
    {
        if(MyPanel.activeSelf)
        {
            MyPanel.SetActive(false);
        }
        else
        {
            MyPanel.SetActive(true);
        }
    }

    public void DungeonEnter()
    {
        FadePanel.Instance.Fade();
        SoundManager.Instance.PlaySFX("Button");
        Invoke("ActiveCtrl", 1f);
    }

    public void DungeonQuit()
    {
        FadePanel.Instance.Fade();
        SoundManager.Instance.PlaySFX("Button");
        Invoke("ActiveCtrl", 1f);
    }
}