using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageRewardDatabase
{
    public string stageName;
    public List<RewardDatabase> rewardDBList;
}

[CreateAssetMenu(fileName = "Reward Data",
    menuName = "Scriptable Object/Stage Reward Data", order = int.MaxValue)]
public class StageReward : ScriptableObject
{
    public List<StageRewardDatabase> rewardDBList = new List<StageRewardDatabase>();
}