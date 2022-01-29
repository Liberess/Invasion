using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public const int HeroMaxSize = 12;

    public bool[] heroUnlockList = new bool[HeroMaxSize];

    //임시 몬스터 데이터 저장
    public Dictionary<string, UnitStatus> heroDic = new Dictionary<string, UnitStatus>();

    //Json 몬스터 데이터 저장
    public List<UnitStatus> heroList = new List<UnitStatus>();

    public int[] heroCostList = new int[HeroMaxSize];
    public List<Sprite> heroSpriteList = new List<Sprite>();
    public List<Sprite> heroSlotSpriteList = new List<Sprite>();
    public List<RuntimeAnimatorController> heroAnimCtrlList = new List<RuntimeAnimatorController>();

    public int heroIndex;

    public List<UnitStatus> partyList = new List<UnitStatus>();
}