using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumalData
{
    public bool isLoadComplete = false;
    public const int HumalMaxSize = 11;

    public bool[] humalUnlockList = new bool[HumalMaxSize];

    //임시 히어로 데이터 저장
    public Dictionary<int, UnitData> humalDic = new Dictionary<int, UnitData>();
    //public Dictionary<string, int> humalPieceAmountDic = new Dictionary<string, int>();
    //public HumalPieceDictionary humalPieceAmountDic = new HumalPieceDictionary();

    //Json 히어로 데이터 저장
    public List<UnitData> humalList = new List<UnitData>();
    public List<UnitData> partyList = new List<UnitData>();
    public List<UnitData> originHumalDataList = new List<UnitData>();
    public List<HumalPiece> humalPieceAmountList = new List<HumalPiece>();

    //[HideInInspector] 
    public List<Sprite> humalSpriteList = new List<Sprite>();
    public SortedDictionary<int, Sprite> humalSpriteDic = new SortedDictionary<int, Sprite>();
    public List<Sprite> humalCardIconList = new List<Sprite>();
    public List<RuntimeAnimatorController> humalAnimCtrlList =
        new List<RuntimeAnimatorController>();
}