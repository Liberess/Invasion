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
    [SerializeField] private int spawnCount = 0;

    [SerializeField] private List<Enemy> spawnedEnemyList = new List<Enemy>();
    [SerializeField] private Queue<Enemy> notSpawnedEnemyQueue= new Queue<Enemy>();

    public UnityAction EnrageAction { get; private set; }

    private void Start()
    {
        dataMgr = DataManager.Instance;
        battleMgr = BattleManager.Instance;

        spawnedEnemyList.Clear();
        notSpawnedEnemyQueue.Clear();
    }

    public void SetupAI()
    {
        spawnDelayTime = 3f;

        var stageNum = int.Parse(dataMgr.GameData.stageInfo.stageNum.Substring(2));
        for (int i = 0; i < stageNum; i++)
        {
            if (spawnDelayTime > 2.2f)
                spawnDelayTime -= 0.1f;
        }

        StartCoroutine(AICo());
        StartCoroutine(EnrageStateCo());
        StartCoroutine(CheckSpawnableCo());
        //StartCoroutine(GetCostCo());
    }

    /// <summary>
    /// EnemyDataList에서 가장 높은 cost를 찾는다.
    /// </summary>
    private int FindHighCost()
    {
        List<UnitData> enemyDataList = new List<UnitData>();

        for (int i = dataMgr.GameData.stageInfo.minEnemyID; i <= dataMgr.GameData.stageInfo.maxEnemyID; i++)
            enemyDataList.Add(dataMgr.GameData.enemyDataList[i]);

        return enemyDataList.Max(e => e.Cost);
    }

    private IEnumerator AICo()
    {
        spawnTime = 0f;

        while (battleMgr.IsPlay)
        {
            if(spawnCount < 5 && spawnTime >= spawnDelayTime)
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
        var enrageTemp = battleMgr.RedBase.HP * 30.0f / 100.0f;

        while (battleMgr.IsPlay)
        {
            if (battleMgr.RedBase.HP <= enrageTemp)
                EnrageAction?.Invoke();

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
                dataMgr.GameData.stageInfo.minEnemyID,
                dataMgr.GameData.stageInfo.maxEnemyID
            );

        ++spawnCount;
        var enemy = battleMgr.InstantiateObj(EUnitQueueType.Enemy).GetComponent<Enemy>();
        enemy.transform.position = battleMgr.RedBase.transform.position;

        if (IsInstantiate())
        {
            spawnedEnemyList.Add(enemy);
        }
        else
        {
            notSpawnedEnemyQueue.Enqueue(enemy);
            enemy.gameObject.SetActive(false);
        }

        dataMgr.GameData.enemyDataList[rand].animCtrl = dataMgr.GameData.enemyAnimCtrlList[rand];
        enemy.UnitSetup(dataMgr.GameData.enemyDataList[rand]);
        EnrageAction += enemy.Enrage;
        enemy.OnDeathAction += () => --spawnCount;
        enemy.OnDeathAction += () => EnrageAction -= enemy.Enrage;
        enemy.OnDeathAction += () => spawnedEnemyList.Remove(enemy);
        enemy.OnDeathAction += () => BattleManager.ReturnObj(EUnitQueueType.Enemy, enemy.gameObject);
    }

    /// <summary>
    /// 만약, Enemy 스폰 위치에 Enemy가 없다면 스폰할 수 있게 한다.
    /// </summary>
    /// <returns></returns>
    private bool IsInstantiate()
    {
        float gap = 0.0f;
        for(int i = 0; i < spawnedEnemyList.Count; i++)
        {
            gap = Vector2.Distance(spawnedEnemyList[i].transform.position, battleMgr.RedBase.transform.position);
            if (spawnedEnemyList[i].transform.position == battleMgr.RedBase.transform.position
                || gap <= 1.0f)
            {
                Debug.Log(spawnedEnemyList[i].name + " gap : " + gap);
                return false;
            }
        }

        return true;
    }

    private IEnumerator CheckSpawnableCo()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);

        while(battleMgr.IsPlay)
        {
            if(notSpawnedEnemyQueue.Count > 0 && IsInstantiate())
            {
                var enemy = notSpawnedEnemyQueue.Dequeue();
                enemy.gameObject.SetActive(true);
                Debug.Log("Active : " + enemy.name);
            }

            yield return waitForSeconds;
        }

        yield return null;
    }
}