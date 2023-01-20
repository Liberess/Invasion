using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public EStageLevel stageLevel;
    public string stageName;
    public string stageNum;
    public int minEnemyID;
    public int maxEnemyID;
    public int enemyMaxCount;
    public UnitData boss;

    public StageInfo(string _name, string _num, EStageLevel _level, int _min, int _max)
    {
        stageName = _name;
        stageNum = _num;
        stageLevel = _level;
        minEnemyID = _min;
        maxEnemyID = _max;
    }
}