using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageLevel
{
    Easy = 0,
    Normal,
    Hard
}

[System.Serializable]
public class StageInfo
{
    public string stageName;
    public StageLevel stageLevel;
    public int minEnemyID;
    public int maxEnemyID;

    public StageInfo(string _name, StageLevel _level, int _min, int _max)
    {
        stageName = _name;
        stageLevel = _level;
        minEnemyID = _min;
        maxEnemyID = _max;
    }
}