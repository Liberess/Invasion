using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public enum Goods
{
    Stamina = 0,
    Gold,
    Dia
}

[System.Serializable]
public class GoodsData
{
    public string name;
    public int count;

    public GoodsData(string _name, int _count)
    {
        name = _name;
        count = _count;
    }
}

[System.Serializable]
public class GameData
{
    public string saveTimeStr;
    public DateTime saveTime;

    public StageInfo stageInfo;
    public Dictionary<string, string> stageNameDic = new Dictionary<string, string>();

    public int[] facilGold = new int[7];
    public int[] facilLevelList = new int[7];
    public bool[] facilUnlockList = new bool[7];
    public float[] facilLimitTime = new float[7];
    public float[] facilSliderTime = new float[7];

    public List<GoodsData> goodsList = new List<GoodsData>();
    [HideInInspector] public string[] goodsNames = { "Stamina", "Gold", "Dia" };

    public int soulGem;

    public float bgm;
    public float sfx;

    public bool isNew;
    public bool isClear;
}