using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleAI : MonoBehaviour
{
    private DataManager dataMgr;
    private BattleManager battleMgr;

    [SerializeField] private float spawnTime = 0f;
    [SerializeField] private float spawnDelayTime = 1f;
    [SerializeField] private float spawnSpeed = 1f;

    public UnityAction EnrageAction { get; private set; }

    private void Start()
    {
        dataMgr = DataManager.Instance;
        battleMgr = BattleManager.Instance;
    }

    public void SetupAI()
    {
        spawnDelayTime = 3f;

        var stageNum = int.Parse(dataMgr.gameData.stageInfo.stageNum.Substring(2));
        for (int i = 0; i < stageNum; i++)
        {
            if (spawnDelayTime > 2.2f)
                spawnDelayTime -= 0.1f;
        }

        StartCoroutine(AICo());
        StartCoroutine(EnrageStateCo());
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
        spawnTime = 0f;

        while (battleMgr.IsPlay)
        {
            if(spawnTime >= spawnDelayTime)
            {
                spawnTime = 0f;
                EnemySpawn();
            }
            else
            {
                spawnTime += Time.deltaTime * spawnSpeed;
            }

            yield return null;
        }

        yield return null;
    }

    private IEnumerator EnrageStateCo()
    {
        var enrageTemp = battleMgr.RedBase.myStat.maxHp * 30.0f / 100.0f;

        while (battleMgr.IsPlay)
        {
            if (battleMgr.RedBase.myStat.hp <= enrageTemp)
                EnrageAction.Invoke();

            yield return null;
        }

        yield return null;
    }

    public void SpawnRush(bool isRush)
    {
        spawnSpeed = (isRush == true) ? spawnSpeed * 1.5f : 1f;
    }

    private void EnemySpawn()
    {
        int rand = Random.Range
            (
                dataMgr.gameData.stageInfo.minEnemyID,
                dataMgr.gameData.stageInfo.maxEnemyID
            );

        var enemy = battleMgr.InstantiateObj(QueueType.Enemy).GetComponent<Enemy>();
        enemy.transform.position = battleMgr.RedBase.transform.position;
        enemy.UnitSetup(new UnitStatus(dataMgr.enemyDataList[rand].myStat));
        EnrageAction += enemy.Enrage;
        enemy.DeathAction += () => EnrageAction -= enemy.Enrage;
        enemy.DeathAction += () => BattleManager.ReturnObj(QueueType.Enemy, enemy.gameObject);
    }
}