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

    public GoodsData[] goods = new GoodsData[3];
    [HideInInspector] public string[] goodsNames = { "Stamina", "Gold", "Dia" };

    public int soulGem;

    public float bgm;
    public float sfx;

    public bool isNew;
    public bool isClear;
}