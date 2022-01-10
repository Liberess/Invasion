using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsData
{
    public bool[] monsUnlockList = new bool[12];

    //임시 몬스터 데이터 저장
    public Dictionary<string, MonsterStatus> monsDic = new Dictionary<string, MonsterStatus>();

    //Json 몬스터 데이터 저장
    public List<MonsterStatus> monsList = new List<MonsterStatus>();

    public int monsIndex;

    public List<Monster> partyList = new List<Monster>();
}