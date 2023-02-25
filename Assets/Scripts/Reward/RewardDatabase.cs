using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Reward
{
    public ERewardType type;
    public int id;
    public int amount;
}

[CreateAssetMenu(fileName = "Reward Data",
    menuName = "Scriptable Object/Reward Data", order = int.MaxValue)]
public class RewardDatabase : ScriptableObject
{
    [SerializeField] private string rewardTag;
    public string RewardTag => rewardTag;
    
    [SerializeField] private List<Reward> rewardList = new List<Reward>();
    public Reward GetReward(int index) => rewardList[index];
    public List<Reward> GetAllReward() => rewardList;
}