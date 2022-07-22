using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public const int HeroMaxSize = 11;

    public bool[] heroUnlockList = new bool[HeroMaxSize];

    //임시 히어로 데이터 저장
    public Dictionary<string, UnitStatus> heroDic = new Dictionary<string, UnitStatus>();

    //Json 히어로 데이터 저장
    public List<UnitStatus> heroList = new List<UnitStatus>();
    public List<UnitStatus> partyList = new List<UnitStatus>();
    public List<UnitStatus> originHeroDataList = new List<UnitStatus>();

    //[HideInInspector] 
    public List<Sprite> heroSpriteList = new List<Sprite>();
    public List<Sprite> heroCardSpriteList = new List<Sprite>();
    public List<RuntimeAnimatorController> heroAnimCtrlList =
        new List<RuntimeAnimatorController>();
}