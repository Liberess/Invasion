using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QueueType
{
    Hero = 0,
    Enemy
}

public class BattleManager : MonoBehaviour
{
    #region Variables
    public static BattleManager Instance { get; private set; }
    private DataManager dataMgr;

    [Header("== Setting Game UI ==")]
    [SerializeField] private Text stageTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text costTxt;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Image leaderImg;

    [Header("== Setting Hero Card =="), Space(10)]
    [SerializeField] private GameObject heroCardGrid;
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private List<GameObject> heroCardList = new List<GameObject>();

    [Header("== Setting Game Variable =="), Space(10)]
    [SerializeField, Range(0f, 3f)] private float getCostDelay = 1f;
    [SerializeField, Range(0, 10)] private int maxCost = 10;
    [SerializeField, Range(0f, 10f)] private float maxLeaderGauge = 10f;
    private float playTime = 0f;
    private int cost = 0;
    private float leaderGauge = 0f;
    public bool isPlay { get; private set; }

    [Header("== Setting Object Pooling =="), Space(10)]
    [SerializeField] private int defaultHeroCount = 20;
    [SerializeField] private int defaultEnemyCount = 20;
    private Dictionary<QueueType, Queue<GameObject>> queDic =
        new Dictionary<QueueType, Queue<GameObject>>();
    private Dictionary<QueueType, GameObject> quePrefabDic =
        new Dictionary<QueueType, GameObject>();
    //private Queue<Hero> heroQueue = new Queue<Hero>();
    //private Queue<Enemy> enemyQueue = new Queue<Enemy>();

    [Header("== Setting Base =="), Space(10)]
    [SerializeField] private GameObject redBase;
    [SerializeField] private GameObject blueBase;

    private Action UpdateCardAction;
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        cost = 0;
        leaderGauge = 0f;
        costSlider.maxValue = maxCost;

        dataMgr = DataManager.Instance;

        quePrefabDic.Add(QueueType.Hero, Resources.Load("Unit/Hero") as GameObject);
        quePrefabDic.Add(QueueType.Enemy, Resources.Load("Unit/Enemy") as GameObject);

        queDic.Clear();
        Initialize(QueueType.Hero, defaultHeroCount);
        Initialize(QueueType.Enemy, defaultEnemyCount);

        SetHeroCard();
        SetCostSlider();
        SetLeaderGaugeImg();

        StartCoroutine(StartCoru());
    }

    private void Update()
    {
        if (isPlay == false)
            return;

        // Set PlayTime
        playTime += Time.deltaTime;
        SetPlayTimeText();
    }

    #region Object Pooling
    private void Initialize(QueueType type, int initCount)
    {
        queDic.Add(type, new Queue<GameObject>());

        for (int i = 0; i < initCount; i++)
            queDic[type].Enqueue(CreateNewObj(type));
    }

    private GameObject CreateNewObj(QueueType type)
    {
        if (!quePrefabDic.ContainsKey(type))
        {
            Debug.Log("해당 " + type + " 타입의 Key가 존재하지 않음");
            return null;
        }

        if(quePrefabDic[type] == null)
        {
            Debug.Log("해당 " + type + " 타입의 Value가 존재하지 않음");
            return null;
        }

        var newObj = Instantiate(quePrefabDic[type].gameObject, transform.position, Quaternion.identity);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static GameObject GetObj(QueueType type)
    {
        if (Instance.queDic[type].Count > 0)
        {
            var obj = Instance.queDic[type].Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObj(type);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnObj(QueueType type, GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.queDic[type].Enqueue(obj);
    }

    private void InstantiateObj(QueueType type, int id = -1)
    {
        var obj = GetObj(type);

        switch (type)
        {
            case QueueType.Hero:
                obj.transform.position = blueBase.transform.position;
                var hero = obj.GetComponent<Hero>();
                hero.UnitSetup(dataMgr.heroData.partyList[id]);
                break;
            case QueueType.Enemy:
                obj.transform.position = redBase.transform.position;
                var enemy = obj.GetComponent<Enemy>();
                break;
            default:
                break;
        }
    }
    #endregion

    private IEnumerator StartCoru()
    {
        yield return new WaitForSeconds(0.5f);

        // Start 텍스트 출력, 애니메이션

        yield return new WaitForSeconds(1.5f);

        // Start 텍스트 종료

        yield return new WaitForSeconds(0.3f);

        // 게임 시작

        isPlay = true;
        StartCoroutine(GetCostCoru());
        StartCoroutine(GetLeaderGaugeCoru());
    }

    #region Get Cost & LeaderGauge
    private IEnumerator GetCostCoru()
    {
        while (isPlay)
        {
            yield return new WaitForSeconds(getCostDelay);

            if (cost < maxCost)
            {
                ++cost;
                SetCostSlider();
            }
        }
    }

    private IEnumerator GetLeaderGaugeCoru()
    {
        while (isPlay)
        {
            yield return new WaitForSeconds(5f);

            if (leaderGauge < maxLeaderGauge)
            {
                ++leaderGauge;
                SetLeaderGaugeImg();
            }
        }
    }
    #endregion

    private void SetHeroCard()
    {
        heroCardList.Clear();

        for (int i = 0; i < dataMgr.heroData.partyList.Count; i++)
        {
            var heroCard = Instantiate(heroCardPrefab, Vector3.zero, Quaternion.identity);
            heroCard.transform.SetParent(heroCardGrid.transform);
            heroCard.transform.localScale = Vector3.one;

            var heroID = dataMgr.heroData.partyList[i].ID;

            // Set Hero Card Sprite
            heroCard.transform.GetChild(0).GetComponent<Image>().sprite =
                dataMgr.heroData.heroCardSpriteList[heroID];

            // Set Hero Cost Text
            heroCard.transform.GetChild(1).GetComponent<Text>().text =
                dataMgr.heroData.heroCostList[heroID].ToString();

            // Set Hero Card OnClick Event
            heroCard.GetComponent<Button>().onClick.AddListener(() => OnClickHeroCard(heroID));
            UpdateCardAction += () => UpdateCardEvent(heroCard, heroID);

            heroCardList.Add(heroCard);
        }
    }

    #region Set UI (PlayTime, Cost, Leader)
    private void SetPlayTimeText()
    {
        int minute = (int)playTime / 60;
        int second = (int)playTime % 60;
        minute %= 60;

        timeTxt.text = string.Format("{0:D2}", minute) + ":" + string.Format("{0:D2}", second);
    }

    private void SetCostSlider()
    {
        UpdateCardAction();
        costSlider.value = cost;
        costTxt.text = cost + "/" + 10;
    }

    private void SetLeaderGaugeImg() =>
        leaderImg.fillAmount = leaderGauge / maxLeaderGauge;

    private void UpdateCardEvent(GameObject target, int id)
    {
        var targetCost = dataMgr.heroData.heroCostList[id];

        var button = target.GetComponent<Button>();
        var lockImg = target.transform.GetChild(2).gameObject;

        if (cost >= targetCost)
        {
            button.interactable = true;
            lockImg.SetActive(false);
        }
        else
        {
            button.interactable = false;
            lockImg.SetActive(true);
        }
    }
    #endregion

    public void OnClickPauseBtn()
    {
        isPlay = false;
    }

    private void OnClickHeroCard(int id)
    {
        var targetCost = dataMgr.heroData.heroCostList[id];

        if (cost >= targetCost)
        {
            cost -= targetCost;
            SetCostSlider();
            InstantiateObj(QueueType.Hero, id);
        }
    }
}