using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    private UnityMainThreadDispatcher dispatcher;

    [Header("== Setting Game UI ==")]
    [SerializeField] private Text stageTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text costTxt;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Image leaderImg;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Text pauseStageInfoTxt;

    [Header("== Setting Hero Card =="), Space(10)]
    [SerializeField] private GameObject heroCardGrid;
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private List<GameObject> heroCardList = new List<GameObject>();

    [Header("== Setting Game Variable =="), Space(10)]
    [SerializeField, Range(0f, 3f)] private float getCostDelay = 1f;
    [SerializeField, Range(0, 10)] private int maxCost = 10;
    public int MaxCost { get => maxCost; }
    [SerializeField, Range(0f, 10f)] private float maxLeaderGauge = 10f;
    private float playTime = 0f;
    private int cost = 0;
    private float leaderGauge = 0f;
    private int starNum = 0;
    public bool IsPlay { get; private set; }

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
    [SerializeField] private Base redBase;
    public Base RedBase { get => redBase; }
    [SerializeField] private Base blueBase;

    [Header("== Setting Game Result =="), Space(10)]
    [SerializeField] private StageReward stageRewardDB;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject rewardGrid;
    [SerializeField] private GameObject rewardSlotPrefab;
    [SerializeField] private GameObject[] conditions = new GameObject[3];
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;
    [SerializeField] private Image startImg;
    [SerializeField] private Sprite[] startSprites = new Sprite[4];

    private Action UpdateCardAction;
    public Action GameOverAction;
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

        if (UnityMainThreadDispatcher.Exists())
            dispatcher = UnityMainThreadDispatcher.Instance();

        GameOverAction += GameOver;
        GameOverAction += InactiveAllHeroCard;

        SetStageInfo();

        InitializeResultUI();

        //redBase.UnitSetup(new UnitStatus("RedBase", 1000));
        //blueBase.UnitSetup(new UnitStatus("BlueBase", 1000));

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
        if (IsPlay == false)
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
            queDic[type].Enqueue(CreateNewObj(type, i));
    }

    private GameObject CreateNewObj(QueueType type, int index = 0)
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
        newObj.name = type.ToString() + index;
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

    public GameObject InstantiateObj(QueueType type)
    {
        var obj = GetObj(type);
        return obj;
    }

    public static void ReturnObj(QueueType type, GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.queDic[type].Enqueue(obj);
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

        IsPlay = true;

        var battleAI = GetComponent<BattleAI>();
        battleAI.SetupAI();

        StartCoroutine(GetCostCoru());
        StartCoroutine(GetLeaderGaugeCoru());
    }

    #region Get Cost & LeaderGauge
    private IEnumerator GetCostCoru()
    {
        WaitForSeconds delay = new WaitForSeconds(getCostDelay);
        while (IsPlay)
        {
            yield return delay;

            if (cost < maxCost)
            {
                ++cost;
                SetCostSlider();
            }
        }
    }

    private IEnumerator GetLeaderGaugeCoru()
    {
        WaitForSeconds delay = new WaitForSeconds(5f);
        while (IsPlay)
        {
            yield return delay;

            if (leaderGauge < maxLeaderGauge)
            {
                ++leaderGauge;
                SetLeaderGaugeImg();
            }
        }
    }
    #endregion

    #region Set UI (PlayTime, Cost, Leader)
    private void SetStageInfo()
    {
        stageTxt.text = dataMgr.GameData.stageInfo.stageNum;

        pauseStageInfoTxt.text = "STAGE " + dataMgr.GameData.stageInfo.stageName
            + "\n" + dataMgr.GameData.stageInfo.stageNum;
    }

    private void SetPlayTimeText()
    {
        int minute = (int)playTime / 60;
        int second = (int)playTime % 60;
        minute %= 60;

        timeTxt.text = string.Format("{0:D2}", minute) + ":" + string.Format("{0:D2}", second);
    }

    private void SetCostSlider()
    {
        dispatcher.Enqueue(UpdateCardAction);
        costSlider.value = cost;
        costTxt.text = cost + "/" + 10;
    }

    private void SetLeaderGaugeImg() =>
        leaderImg.fillAmount = leaderGauge / maxLeaderGauge;

    private void UpdateCardEvent(GameObject target, int index)
    {
        if (index < 0 || index >= dataMgr.HeroData.partyList.Count)
            throw new Exception("UpdateCardEvent - 잘못된 index값입니다.");

        var targetCost = dataMgr.HeroData.partyList[index].cost;

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

    private void SetHeroCard()
    {
        heroCardList.Clear();

        for (int i = 0; i < dataMgr.HeroData.partyList.Count; i++)
        {
            var heroCard = Instantiate(heroCardPrefab, Vector3.zero, Quaternion.identity);
            heroCard.transform.SetParent(heroCardGrid.transform);
            heroCard.transform.localScale = Vector3.one;

            var heroID = dataMgr.HeroData.partyList[i].ID;

            // Set Hero Card Sprite
            heroCard.transform.GetChild(0).GetComponent<Image>().sprite =
                dataMgr.HeroData.heroCardSpriteList[heroID];

            // Set Hero Cost Text
            heroCard.transform.GetChild(1).GetComponent<Text>().text =
                dataMgr.HeroData.partyList[i].cost.ToString();

            // Set Hero Card OnClick Event
            int index = i;
            heroCard.GetComponent<Button>().onClick.AddListener(() => OnClickHeroCard(index));
            UpdateCardAction += () => UpdateCardEvent(heroCard, index);

            heroCardList.Add(heroCard);
        }
    }

    private void InactiveAllHeroCard()
    {
        foreach(var card in heroCardList)
        {
            var button = card.GetComponent<Button>();
            button.interactable = false;

            var lockImg = card.transform.GetChild(2).gameObject;
            lockImg.SetActive(true);
        }
    }
    #endregion

    #region Result
    public void GameOver()
    {
        if (!IsPlay)
            return;

        Debug.Log("GameOver");

        IsPlay = false;

        resultPanel.SetActive(true);

        if (redBase.Hp <= 0)
            GameVictory();
        else if (blueBase.Hp <= 0)
            GameDefeat();
    }

    public void GameDefeat()
    {
        defeatPanel.SetActive(true);
        Debug.Log("GameDefeat");
    }

    public void GameVictory()
    {
        GetReward();
        SetupConditions();

        victoryPanel.SetActive(true);
        Debug.Log("GameVictory");
    }

    private void InitializeResultUI()
    {
        resultPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        for (int i = 0; i < rewardGrid.transform.childCount; i++)
            Destroy(rewardGrid.transform.GetChild(i).gameObject);
    }

    private void SetupConditions()
    {
        starNum = 0;

        if (blueBase.Hp >= 10)
        {
            ++starNum;
            conditions[0].GetComponentInChildren<Image>().sprite = starOn;
        }
        else
        {
            conditions[0].GetComponentInChildren<Image>().sprite = starOff;
        }

        if (blueBase.Hp >= 50)
        {
            ++starNum;
            conditions[1].GetComponentInChildren<Image>().sprite = starOn;
        }
        else
        {
            conditions[1].GetComponentInChildren<Image>().sprite = starOff;
        }

        if (playTime < 180)
        {
            ++starNum;
            conditions[2].GetComponentInChildren<Image>().sprite = starOn;
        }
        else
        {
            conditions[2].GetComponentInChildren<Image>().sprite = starOff;
        }

        startImg.sprite = startSprites[starNum];
    }

    private void GetReward()
    {
        var rewards = GetRewardDatabase();

        for (int i = 0; i < rewards.Length; i++)
        {
            var reward = Instantiate(rewardSlotPrefab);
            reward.transform.SetParent(rewardGrid.transform);
            reward.transform.localScale = new Vector3(1, 1, 1);
            reward.transform.GetChild(0).GetComponent<Image>().sprite =
                dataMgr.GameData.goodsSpriteList[(int)rewards[i].type];
            reward.GetComponentInChildren<Text>().text = rewards[i].num.ToString();

            dataMgr.SetGoods(rewards[i].type, rewards[i].num);
        }
    }

    private Reward[] GetRewardDatabase()
    {
        int stageIndex = int.Parse(dataMgr.GameData.stageInfo.stageNum.Split('-')[0]);
        int levelIndex = int.Parse(dataMgr.GameData.stageInfo.stageNum.Split('-')[1]);
        return stageRewardDB.rewardDBList[stageIndex - 1].
            rewardDBList[levelIndex - 1].GetAllReward();
    }
    #endregion

    #region OnClick Events
    public void OnClickPauseBtn(bool isPause)
    {
        if(isPause)
        {
            IsPlay = false;
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            IsPlay = true;
            pausePanel.SetActive(false);
        }
    }

    public void OnClickRetryBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickQuitBtn()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void OnClickHeroCard(int index)
    {
        var targetCost = dataMgr.HeroData.partyList[index].cost;

        if (cost >= targetCost)
        {
            cost -= targetCost;
            SetCostSlider();

            var hero = InstantiateObj(QueueType.Hero).GetComponent<Hero>();
            hero.transform.position = blueBase.transform.position;
            hero.UnitSetup(dataMgr.HeroData.partyList[index]);
            hero.DeathAction += () => ReturnObj(QueueType.Hero, hero.gameObject);
        }
    }
    #endregion
}