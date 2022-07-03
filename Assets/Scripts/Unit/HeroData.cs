using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public const int HeroMaxSize = 12;

    public bool[] heroUnlockList = new bool[HeroMaxSize];
    public int[] heroCostList = new int[HeroMaxSize];
    public int[] heroGoldList = new int[HeroMaxSize];
    public int[] heroSoulGemList = new int[HeroMaxSize];

    //임시 히어로 데이터 저장
    public Dictionary<string, UnitStatus> heroDic = new Dictionary<string, UnitStatus>();

    //Json 히어로 데이터 저장
    public List<UnitStatus> heroList = new List<UnitStatus>();
    public List<UnitStatus> partyList = new List<UnitStatus>();

    [HideInInspector] public List<Sprite> heroSpriteList = new List<Sprite>();
    [HideInInspector] public List<Sprite> heroCardSpriteList = new List<Sprite>();
    [HideInInspector] public List<RuntimeAnimatorController> heroAnimCtrlList =
        new List<RuntimeAnimatorController>();
}