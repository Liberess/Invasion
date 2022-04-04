using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    private DataManager dataMgr;
    private BattleManager battleMgr;

    private bool isHighEnemy;
    private float getCostDelay = 1f;

    private void Start()
    {
        dataMgr = DataManager.Instance;
        battleMgr = BattleManager.Instance;
    }

    public void SetupAI()
    {
        getCostDelay = 3f;

        var stageNum = int.Parse(dataMgr.gameData.stageInfo.stageNum.Substring(2));
        for (int i = 0; i < stageNum; i++)
        {
            if (getCostDelay > 2.2f)
                getCostDelay -= 0.1f;
        }

        StartCoroutine(AICo());
        //StartCoroutine(GetCostCo());
    }

    /// <summary>
    /// EnemyDataList에서 가장 높은 cost를 찾는다.
    /// </summary>
    private int FindHighCost()
    {
        List<EnemyData> enemyDataList = new List<EnemyData>();

        for (int i = dataMgr.gameData.stageInfo.minEnemyID; i <= dataMgr.gameData.stageInfo.maxEnemyID; i++)
            enemyDataList.Add(dataMgr.enemyDataList[i]);

        enemyDataList = enemyDataList.OrderByDescending(x => x.myStat.cost).ToList();
        return enemyDataList[0].myStat.cost;
    }

    private IEnumerator AICo()
    {
        WaitForSeconds delay = new WaitForSeconds(getCostDelay);

        while (battleMgr.IsPlay)
        {
            EnemySpawn();

            yield return delay;
        }

        yield return null;
    }

    private IEnumerator GetCostCo()
    {
        WaitForSeconds delay = new WaitForSeconds(getCostDelay);

        while (battleMgr.IsPlay)
        {
            yield return delay;

/*            if (cost < maxCost)
                ++cost;*/
        }
    }

    private void EnemySpawn()
    {
        int rand = Random.Range
            (
                dataMgr.gameData.stageInfo.minEnemyID,
                dataMgr.gameData.stageInfo.maxEnemyID
            );

        // enemy 생성
        Debug.Log("enemy 생성!");
        var enemy = battleMgr.InstantiateObj(QueueType.Enemy, rand).GetComponent<Enemy>();
    }
}