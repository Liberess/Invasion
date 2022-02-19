using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

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

    public int dia;
    public int gold;
    public int soulGem;
    public int drink;

    public float bgm;
    public float sfx;

    public bool isNew;
    public bool isClear;
}