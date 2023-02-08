using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Currency
{
    public string name;
    public int count;

    public Currency(string _name, int _count)
    {
        name = _name;
        count = _count;
    }
}

[System.Serializable]
public class GameData
{
    public bool isLoadComplete = false;
    public string lastLogInTimeStr;
    public DateTime lastLogInTime;

    public Dictionary<string, Item> inventoryDic = new Dictionary<string, Item>();

    public StageInfo stageInfo;
    public Dictionary<string, string> stageNameDic = new Dictionary<string, string>();

    public GameObject[] unitPrefabAry = new GameObject[2];
    public GameObject lobbyHumalPrefab;

    public List<UnitData> enemyDataList = new List<UnitData>();
    public List<Sprite> enemySpriteList = new List<Sprite>();
    public List<RuntimeAnimatorController> enemyAnimCtrlList = new List<RuntimeAnimatorController>();

    public int[] facilGold = new int[7];
    public int[] facilLevelList = new int[7];
    public bool[] facilUnlockList = new bool[7];
    public float[] facilLimitTime = new float[7];
    public float[] facilSliderTime = new float[7];

    public float bgm;
    public float sfx;

    public bool isNew;
}