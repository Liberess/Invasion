using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public bool isLoadComplete = false;
    public const int HeroMaxSize = 11;

    public bool[] heroUnlockList = new bool[HeroMaxSize];

    //임시 히어로 데이터 저장
    public Dictionary<int, HumalData> heroDic = new Dictionary<int, HumalData>();
    //public Dictionary<string, int> humalPieceAmountDic = new Dictionary<string, int>();
    public HumalPieceDictionary humalPieceAmountDic = new HumalPieceDictionary();

    //Json 히어로 데이터 저장
    public List<HumalData> heroList = new List<HumalData>();
    public List<HumalData> partyList = new List<HumalData>();
    public List<UnitDataSO> originHeroDataList = new List<UnitDataSO>();
    [HideInInspector] public List<HumalPiece> humalPieceAmountList = new List<HumalPiece>();

    //[HideInInspector] 
    public List<Sprite> heroSpriteList = new List<Sprite>();
    public List<Sprite> heroCardSpriteList = new List<Sprite>();
    public List<RuntimeAnimatorController> heroAnimCtrlList =
        new List<RuntimeAnimatorController>();
}