using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public bool[] heroUnlockList = new bool[12];

    //임시 몬스터 데이터 저장
    public Dictionary<string, UnitStatus> heroDic = new Dictionary<string, UnitStatus>();

    //Json 몬스터 데이터 저장
    public List<UnitStatus> heroList = new List<UnitStatus>();

    public int[] heroCostList = new int[12];
    public List<Sprite> heroSpriteList = new List<Sprite>();
    public List<Sprite> heroSlotSpriteList = new List<Sprite>();
    public List<RuntimeAnimatorController> heroAnimCtrlList = new List<RuntimeAnimatorController>();

    public int heroIndex;

    public List<Hero> partyList = new List<Hero>();
}