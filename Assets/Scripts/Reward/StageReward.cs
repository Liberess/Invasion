using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct StageRewardDatabase
{
    public string stageName;
    public List<RewardDatabase> rewardList;

    public StageRewardDatabase(string name, List<RewardDatabase> list)
    {
        stageName = name;
        rewardList = list;
    }
}

[CreateAssetMenu(fileName = "Reward Data",
    menuName = "Scriptable Object/Stage Reward Data", order = int.MaxValue)]
public class StageReward : ScriptableObject
{
    [SerializeField] private List<string> rewardPathList = new List<string>();
    public List<StageRewardDatabase> rewardDBList = new List<StageRewardDatabase>();

    public bool GetRewardsOfTag(string tag, out List<Reward> rewards)
    {
        List<Reward> tempRewards = new List<Reward>();

        rewardDBList.Find(x => x.rewardList.Find(y =>
        {
            tempRewards = y.GetAllReward();
            return y.RewardTag == tag;
        }));

        if (tempRewards.Count > 0)
        {
            rewards = tempRewards;
            return true;
        }
        
        rewards = null;
        return false;
    }

    [ContextMenu("Update Reward DB")]
    public void UpdateRewardDB()
    {
        #if UNITY_EDITOR_WIN
        rewardDBList.Clear();

        for (int i = 0; i < rewardPathList.Count; i++)
        {
            var assetsPath = new string[] { string.Concat("Assets/Scriptables/StageReward/", rewardPathList[i]) };
            var guids = AssetDatabase.FindAssets("t:ScriptableObject", assetsPath);
            List<RewardDatabase> tempRewardList = new List<RewardDatabase>();

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                if (obj is RewardDatabase)
                    tempRewardList.Add(obj as RewardDatabase);
            }

            rewardDBList.Add(new StageRewardDatabase(rewardPathList[i], tempRewardList));
        }
        #endif
    }
}