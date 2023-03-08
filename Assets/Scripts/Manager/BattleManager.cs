using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

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
    private Dictionary<EUnitQueueType, Queue<GameObject>> queDic =
        new Dictionary<EUnitQueueType, Queue<GameObject>>();
    private Dictionary<EUnitQueueType, GameObject> quePrefabDic =
        new Dictionary<EUnitQueueType, GameObject>();

    [Header("== Setting Base =="), Space(10)]
    [SerializeField] private Base redBase;
    public Base RedBase { get => redBase; }
    [SerializeField] private Base blueBase;

    [Header("== Setting Game Result =="), Space(10)] 
    [SerializeField] private AssetReference stageRewardRuleRef;
    private StageRewardRuleDB stageRewardRuleDB;
    [SerializeField] private StageReward stageRewardDB;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject rewardGrid;
    [SerializeField] private GameObject rewardSlotPrefab;
    [SerializeField] private Image[] conditionImgs = new Image[3];
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;
    [SerializeField] private Image[] starImgs = new Image[3];

    private Action UpdateCardAction;
    public Action GameOverAction;

    private AsyncOperationHandle updateBundleHandle;

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
        dataMgr = DataManager.Instance;
        InitializeAsync().Forget();
    }

    private void Update()
    {
        if (IsPlay == false)
            return;

        // Set PlayTime
        playTime += Time.deltaTime;
        SetPlayTimeText();
    }

    private async UniTaskVoid InitializeAsync()
    {
        await UniTask.WaitUntil(() => (stageRewardRuleDB = ResourcesManager.Instance.LoadAsset<StageRewardRuleDB>(stageRewardRuleRef) as StageRewardRuleDB) != null);
        
        cost = 0;
        leaderGauge = 0f;
        costSlider.maxValue = maxCost;

        GameOverAction += GameOver;
        GameOverAction += InactiveAllHeroCard;

        SetStageInfo();
        InitializeResultUI();

        queDic.Clear();
        quePrefabDic.Clear();
        
        quePrefabDic.Add(EUnitQueueType.Hero, dataMgr.UnitPrefabAry[(int)EUnitQueueType.Hero]);
        quePrefabDic.Add(EUnitQueueType.Enemy, dataMgr.UnitPrefabAry[(int)EUnitQueueType.Enemy]);
        Initialize(EUnitQueueType.Hero, defaultHeroCount);
        Initialize(EUnitQueueType.Enemy, defaultEnemyCount);

        /*bool isLoadComplete = false;
        Addressables.LoadAssetAsync<GameObject>(heroReference).Completed +=
            handle =>
            {
                isLoadComplete = true;
                if (!quePrefabDic.ContainsKey(EUnitQueueType.Hero))
                {
                    quePrefabDic.Add(EUnitQueueType.Hero, handle.Result);
                    Initialize(EUnitQueueType.Hero, defaultHeroCount);
                }
            };

        await UniTask.WaitUntil(() => isLoadComplete == true);

        isLoadComplete = false;
        Addressables.LoadAssetAsync<GameObject>(enemyReference).Completed +=
            handle =>
            {
                isLoadComplete = true;
                if (!quePrefabDic.ContainsKey(EUnitQueueType.Enemy))
                {
                    quePrefabDic.Add(EUnitQueueType.Enemy, handle.Result);
                    Initialize(EUnitQueueType.Enemy, defaultEnemyCount);
                }
            };

        await UniTask.WaitUntil(() => isLoadComplete == true);*/

        SetHeroCard();
        SetCostSlider();
        SetLeaderGaugeImg();

        StartCoroutine(StartCoru());
    }

    #region Object Pooling
    private void Initialize(EUnitQueueType type, int initCount)
    {
        queDic.Add(type, new Queue<GameObject>());

        for (int i = 0; i < initCount; i++)
            queDic[type].Enqueue(CreateNewObj(type, i));
    }

    private GameObject CreateNewObj(EUnitQueueType type, int index = 0)
    {
        if (!quePrefabDic.ContainsKey(type))
        {
            Debug.Log("해당 " + type + " 타입의 Key가 존재하지 않음");
            return null;
        }

        if(!quePrefabDic[type])
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

    public static GameObject GetObj(EUnitQueueType type)
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

    public GameObject InstantiateObj(EUnitQueueType type)
    {
        var obj = GetObj(type);
        return obj;
    }

    public static void ReturnObj(EUnitQueueType type, GameObject obj)
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
        stageTxt.text = dataMgr.CurrentStageInfo.stageNum;

        pauseStageInfoTxt.text = "STAGE " + dataMgr.CurrentStageInfo.name_ko
            + "\n" + dataMgr.CurrentStageInfo.stageNum;
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
        UpdateCardAction?.Invoke();
        costSlider.value = cost;
        costTxt.text = cost + "/" + 10;
    }

    private void SetLeaderGaugeImg() =>
        leaderImg.fillAmount = leaderGauge / maxLeaderGauge;

    private void UpdateCardEvent(GameObject target, int index)
    {
        if (index < 0 || index >= dataMgr.HumalData.partyList.Count)
            throw new Exception("UpdateCardEvent - 잘못된 index값입니다.");

        var targetCost = dataMgr.HumalData.partyList[index].Cost;

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

        for (int i = 0; i < dataMgr.HumalData.partyList.Count; i++)
        {
            var heroCard = Instantiate(heroCardPrefab, Vector3.zero, Quaternion.identity);
            heroCard.transform.SetParent(heroCardGrid.transform);
            heroCard.transform.localScale = Vector3.one;

            var heroID = dataMgr.HumalData.partyList[i].ID;

            // Set Hero Card Sprite
            heroCard.transform.GetChild(0).GetComponent<Image>().sprite =
                dataMgr.HumalData.GetHumalCardIcon(heroID);

            // Set Hero Cost Text
            heroCard.transform.GetChild(1).GetComponent<Text>().text =
                dataMgr.HumalData.partyList[i].Cost.ToString();

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

        if (redBase.HP <= 0)
            GameVictory();
        else if (blueBase.HP <= 0)
            GameDefeat();
    }

    public void GameDefeat()
    {
        defeatPanel.SetActive(true);
        Debug.Log("GameDefeat");
    }

    public void GameVictory()
    {
        SetupConditions();
        GetReward();

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

        /*for (int i = 0; i < conditionImgs.Length; i++)
            conditionImgs[i].sprite = starOff;*/
        
        if (blueBase.HP >= 10)
            ++starNum;

        if (blueBase.HP >= 50)
            ++starNum;

        if (playTime < 180)
            ++starNum;

        for (int i = 0; i < conditionImgs.Length; i++)
        {
            if (starNum > i)
            {
                starImgs[i].sprite = starOn;
                conditionImgs[i].sprite = starOn;
            }
            else
            {
                starImgs[i].sprite = starOff;
                conditionImgs[i].sprite = starOff;
            }
        }
    }

    private void GetReward()
    {
        var stageData = dataMgr.GameData.stageDataList.Find(e => e.stageNum == dataMgr.CurrentStageInfo.stageNum);
        
        for (int i = 0; i < stageRewardRuleDB.RuleList.Count; i++)
        {
            var entity = stageRewardRuleDB.RuleList[i];
            int[] rules = { entity.star1, entity.star2, entity.star3 };
            if (Enum.TryParse(entity.type, out ECurrencyType type))
            {
                if (type == ECurrencyType.DA && stageData.IsAllClear)
                    continue;

                int amount = 0;
                switch (type)
                {
                    case ECurrencyType.GD:
                        int stageIndex = int.Parse(dataMgr.CurrentStageInfo.stageNum.Split('-')[0]);
                        int levelIndex = int.Parse(dataMgr.CurrentStageInfo.stageNum.Split('-')[1]);
                    
                        amount = (stageIndex * 1000) + (levelIndex * 100) + Random.Range(0, 100);
                        if (starNum > 0)
                            amount *= (1 + rules[starNum - 1] % 100);
                        break;
                    
                    case ECurrencyType.DA:
                        if (starNum > stageData.StarAmount)
                        {
                            for (int j = 0; j < starNum; j++)
                            {
                                if (stageData.isStar[j] == false)
                                {
                                    stageData.isStar[j] = true;
                                    amount += rules[j];
                                }
                            }
                        }
                        break;
                    
                    case ECurrencyType.AJ:
                        if (starNum == 1)
                        {
                            amount = entity.star1;
                        }
                        else if (starNum == 2)
                        {
                            var picker = new Rito.WeightedRandomPicker<int>();
                            picker.Add(
                                (entity.star1, 50),
                                (entity.star1 * 2, 35),
                                (entity.star3, 15)
                            );

                            amount = picker.GetRandomPick();
                        }
                        else if(starNum == 3)
                        {
                            var picker = new Rito.WeightedRandomPicker<int>();
                            picker.Add(
                                (entity.star1, 30),
                                (entity.star1 * 2, 30),
                                (entity.star1 * 3, 20),
                                (entity.star1 * 4, 15),
                                (entity.star3, 5)
                            );
                            
                            amount = picker.GetRandomPick();
                        }
                        break;
                }

                dataMgr.SetCurrencyAmount(type, amount);
                InstantiateReward(dataMgr.goodsSpriteList[(int)type], amount.ToString());
            }
        }

        if (stageRewardDB.GetRewardsOfTag(dataMgr.CurrentStageInfo.stageNum, out List<Reward> rewards))
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                string amountStr = "";
                if(rewards[i].amount > 1)
                    amountStr = rewards[i].amount.ToString();

                Sprite rewardImg = null;
                if (rewards[i].type == ERewardType.HP)
                {
                    dataMgr.AddHumalPiece(rewards[i].id, rewards[i].amount);
                
                    /*reward.transform.GetChild(0).GetComponent<Image>().sprite =
                        dataMgr.goodsSpriteList[(int)rewards[i].type];*/
                }
                else
                {
                    ECurrencyType type = (ECurrencyType)Enum.Parse(typeof(ECurrencyType), rewards[i].type.ToString());
                    dataMgr.SetCurrencyAmount(type, rewards[i].amount);

                    rewardImg = dataMgr.goodsSpriteList[(int)rewards[i].type];
                }

                InstantiateReward(rewardImg, amountStr);
            }
        }
    }

    private void InstantiateReward(Sprite sprite, string txt)
    {
        var reward = Instantiate(rewardSlotPrefab);
        reward.transform.SetParent(rewardGrid.transform);
        reward.transform.localScale = new Vector3(1, 1, 1);

        reward.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        reward.GetComponentInChildren<Text>().text = txt;
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
        dataMgr.LoadScene("Lobby").Forget();
    }

    private void OnClickHeroCard(int index)
    {
        var targetCost = dataMgr.HumalData.partyList[index].Cost;

        if (cost >= targetCost)
        {
            cost -= targetCost;
            SetCostSlider();

            var hero = InstantiateObj(EUnitQueueType.Hero).GetComponent<Humal>();
            hero.transform.position = blueBase.transform.position;
            hero.UnitSetup(dataMgr.HumalData.partyList[index]);
            hero.OnDeathAction += () => ReturnObj(EUnitQueueType.Hero, hero.gameObject);
        }
    }
    #endregion
}