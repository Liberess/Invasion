using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public int index;
    public string name_ko;
    public string name_en;
    public EStageLevel stageLevel = EStageLevel.Easy;
    public string stageNum = "";
    public int minEnemyID = 0;
    public int maxEnemyID = 0;
    public int enemyMaxCount = 5;
    public UnitData boss = null;
    public bool[] isStar = new bool[3] { false, false, false };
    public bool IsAllClear => !isStar.Contains(false);
    public int StarAmount => Array.FindAll(isStar, e => e == true).Length;
}