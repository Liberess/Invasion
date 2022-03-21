using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    private DataManager dataMgr;
    private BattleManager battleMgr;

    [SerializeField] private int cost = 0;
    private int highCost = 0;
    private bool isHighEnemy;
    private int maxCost;
    private float getCostDelay = 1f;

    private void Start()
    {
        dataMgr = DataManager.Instance;
        battleMgr = BattleManager.Instance;
    }

    public void SetupAI()
    {
        maxCost = battleMgr.MaxCost;

        getCostDelay = 3f;

        var stageNum = int.Parse(dataMgr.gameData.stageInfo.stageNum.Substring(2));
        for (int i = 0; i < stageNum; i++)
        {
            if (getCostDelay > 2.2f)
                getCostDelay -= 0.1f;
        }

        highCost = FindHighCost();

        StartCoroutine(AICo());
        StartCoroutine(GetCostCo());
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
        while(battleMgr.isPlay)
        {
            EnemySpawn();

            float randDelay = Random.Range(1f, 3f);
            yield return new WaitForSeconds(randDelay);
        }

        yield return null;
    }

    private IEnumerator GetCostCo()
    {
        WaitForSeconds delay = new WaitForSeconds(getCostDelay);

        while (battleMgr.isPlay)
        {
            yield return delay;

            if (cost < maxCost)
                ++cost;
        }
    }

    private void EnemySpawn()
    {
        int rand = Random.Range
            (
                dataMgr.gameData.stageInfo.minEnemyID,
                dataMgr.gameData.stageInfo.maxEnemyID
            );

        // 만약 cost가 부족하다면 종료
        if (cost < dataMgr.enemyDataList[rand].myStat.cost)
        {
            Debug.Log("코스트 부족!");
            return;
        }

        // 만약 생성된 enemy가 가장 높은 cost라면
        if (dataMgr.enemyDataList[rand].myStat.cost == highCost && isHighEnemy)
        {
            Debug.Log("이미 높은 cost의 적군 생성함!");
            isHighEnemy = false;
            EnemySpawn();
            return;
        }
        else if(dataMgr.enemyDataList[rand].myStat.cost == highCost && !isHighEnemy)
        {
            Debug.Log("높은 cost 적군 생성!");
            isHighEnemy = true;

            // enemy 생성
            var enemy = battleMgr.InstantiateObj(QueueType.Enemy, rand).GetComponent<Enemy>();
            cost -= enemy.myStat.cost;
        }
        else
        {
            // enemy 생성
            Debug.Log("일반 enemy 생성!");
            var enemy = battleMgr.InstantiateObj(QueueType.Enemy, rand).GetComponent<Enemy>();
            cost -= enemy.myStat.cost;
        }
    }
}