using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Reward
{
    public GoodsType type;
    public int num;
}

[CreateAssetMenu(fileName = "Reward Data",
    menuName = "Scriptable Object/Reward Data", order = int.MaxValue)]
public class RewardDatabase : ScriptableObject
{
    [SerializeField] private Reward[] rewards;
    public int rewardCount => rewards.Length;

    public Reward GetReward(int index) => rewards[index];
    public Reward[] GetAllReward() => rewards;
}