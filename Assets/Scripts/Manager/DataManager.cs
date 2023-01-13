using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Linq;

public class DataManager : MonoBehaviour
{
    private UIManager uiMgr;
    private PopUpManager popUpMgr;

    [Header("==== Hero Object Prefab ====")]
    [SerializeField] private AssetReference heroReference;
    [SerializeField] private AssetReference lobbyHeroReference;
    [SerializeField] private List<UnitData> lobbyHeroList = new List<UnitData>();
    //[SerializeField] private GameObject heroPrefab;
    //[SerializeField] private GameObject lobbyHeroPrefab;

    public UnitData LeaderHero { get; private set; }

    #region 인스턴스화
    public static DataManager Instance { get; private set; }

    [Header("==== Game Data Information ===="), Space(10)]
    [SerializeField] private GameData m_GameData;
    public GameData GameData { get => m_GameData; }

    [Header("==== Hero Data Information ===="), Space(10)]
    [SerializeField] private HeroData m_HeroData;
    public HeroData HeroData { get => m_HeroData; }

    [Header("==== Enemy Data Information ===="), Space(10)]
    [SerializeField] private List<UnitData> m_EnemyDataList = new List<UnitData>();
    public List<UnitData> EnemyDataList { get => m_EnemyDataList; }
    [SerializeField] private List<Sprite> m_EnemySpriteList = new List<Sprite>();
    public List<Sprite> EnemySpriteList => m_EnemySpriteList;

    [Header("==== Item Data Information ===="), Space(10)]
    [SerializeField] private List<ItemData> mItemDataList = new List<ItemData>();
    public List<ItemData> ItemDataList => mItemDataList;
    #endregion

    private Dictionary<string, int> VirtualCurrencyDic = new Dictionary<string, int>();

    public List<Sprite> goodsSpriteList = new List<Sprite>();

    public UnityAction OnDataLoadSuccessAction;

    private bool isDownStart = false;
    [SerializeField] private Text isDownTxt;
    [SerializeField] private Text isProgressTxt;

    private AsyncOperationHandle updateBundleHandle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        StartCoroutine(UpdateResources());
    }

    private void Start()
    {
        uiMgr = UIManager.Instance;
        popUpMgr = PopUpManager.Instance;

        GameManager.Instance.OnApplicationStart();

        OnDataLoadSuccessAction = null;
    }

    public void SetStageInfo(StageInfo info) => GameData.stageInfo = info;

    #region Update Resources
    [ContextMenu("Update Resources")]
    public IEnumerator UpdateResources()
    {
        UpdateGoodsSprite();
        UpdateHeroSprite();
        UpdateOriginDB();
        UpdateHeroAnimCtrl();
        UpdateHeroCardSprite();
        UpdateItemData();

        yield return null;
    }

    private void UpdateGoodsSprite()
    {
        goodsSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("Goods");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                goodsSpriteList.Add(temp[i]);
        }
    }

    private void UpdateOriginDB()
    {
        HeroData.originHeroDataList.Clear();
        /*
                updateBundleHandle = Addressables.LoadAssetsAsync<UnitData>(
                    Tags.HumalDataLabel,
                    (result) =>
                    {
                        if (!isDownStart)
                        {
                            isDownStart = true;
                            isDownTxt.text = "기본 휴멀 데이터 다운로드 시작";
                            StartCoroutine(UpdateBundleProgressTxtCo());
                        }

                        if (!m_HeroData.originHeroDataList.Contains(result))
                            m_HeroData.originHeroDataList.Add(result);
                    }
                );*/


        updateBundleHandle = Addressables.LoadAssetsAsync<UnitOriginDB>(
            Tags.UnitOriginDBLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "유닛 DB 다운로드 시작";
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                foreach(var unitDB in result.HumalDBList)
                {
                    if (!m_HeroData.originHeroDataList.Contains(unitDB))
                        m_HeroData.originHeroDataList.Add(unitDB);
                }

                foreach(var unitDB in result.EnemyDBList)
                {
                    if(!m_EnemyDataList.Contains(unitDB))
                        m_EnemyDataList.Add(unitDB);
                }
            }
        );

        updateBundleHandle = Addressables.LoadAssetsAsync<Sprite>(
            Tags.EnemyLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "몬스터 스프라이트 다운로드 시작";
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if (!m_EnemySpriteList.Contains(result))
                    m_EnemySpriteList.Add(result);
            }
        );

        if (updateBundleHandle.IsValid())
        {
            updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
            {
                updateBundleHandle = handle;
                isDownTxt.text = "다운로드 완료";
            };
        }
        else
        {
            Debug.LogWarning("updateBundleHandle is not valid!!");
        }

    }

    //[ContextMenu("Update Hero Sprite")]
    private void UpdateHeroSprite()
    {
        HeroData.heroSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("HeroSprite");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroSpriteList.Add(temp[i]);
        }
    }

    //[ContextMenu("Update Hero Card Sprite")]
    private void UpdateHeroCardSprite()
    {
        HeroData.heroCardSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("HeroCardSprite");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroCardSpriteList.Add(temp[i]);
        }
    }

    //[ContextMenu("Update Hero Anim Controller")]
    private void UpdateHeroAnimCtrl()
    {
        HeroData.heroAnimCtrlList.Clear();

        RuntimeAnimatorController[] temp =
            Resources.LoadAll<RuntimeAnimatorController>("HeroAnimCtrl");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroAnimCtrlList.Add(temp[i]);
        }
    }

    private IEnumerator UpdateBundleProgressTxtCo()
    {
        yield return null;

        while (true)
        {
            if (updateBundleHandle.PercentComplete >= 1.0f)
                break;

            isProgressTxt.text = string.Concat(
                (updateBundleHandle.PercentComplete * 100.0f).ToString(), "%");
            yield return null;
        }
    }

    private void UpdateItemData()
    {
        mItemDataList.Clear();

        ItemData[] temp = Resources.LoadAll<ItemData>("Scriptable/ItemData");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                mItemDataList.Add(temp[i]);
        }
    }
    #endregion

    #region Hero List - Dic
    private void ConstructHeroDic()
    {
        m_HeroData.heroDic = new Dictionary<int, UnitData>();
        m_HeroData.heroDic.Clear();

        //Json에 저장된 heroList안의 내용들을 heroDic에 저장
        for (int i = 0; i < m_HeroData.heroList.Count; i++)
            m_HeroData.heroDic.Add(m_HeroData.heroList[i].ID, m_HeroData.heroList[i]);
    }

    private void ResettingHeroList()
    {
        m_HeroData.heroList.Clear();
        m_HeroData.heroList = new List<UnitData>(m_HeroData.heroDic.Values);
    }

    private void ConstructHumalPieceAmountDic()
    {
        //m_HeroData.humalPieceAmountDic = new Dictionary<string, int>();
        m_HeroData.humalPieceAmountDic = new HumalPieceDictionary();

        for (int i = 0; i < m_HeroData.humalPieceAmountList.Count; i++)
        {
            m_HeroData.humalPieceAmountDic.Add(
                m_HeroData.humalPieceAmountList[i].name,
                m_HeroData.humalPieceAmountList[i].amount
            );
        }
    }

    private void ResettingHumalPieceAmountList()
    {
        m_HeroData.humalPieceAmountList.Clear();

        List<HumalPiece> temList = new List<HumalPiece>();
        foreach(var data in m_HeroData.humalPieceAmountDic)
            temList.Add(new HumalPiece(data.Key, data.Value));

        m_HeroData.humalPieceAmountList = temList;
    }

    /// <summary>
    /// HumalList에 id가 존재하는지 확인한다.
    /// </summary>
    public bool IsContainsInHumalList(int id)
    {
        foreach (var heroData in m_HeroData.heroList)
        {
            if (heroData.ID == id)
                return true;
        }

        return false;
    }

    /// <summary>
    /// PartyList에 id가 존재하는지 확인한다.
    /// </summary>
    public bool IsContainsInParty(int id)
    {
        foreach (var HeroData in m_HeroData.partyList)
        {
            if (HeroData.ID == id)
                return true;
        }

        return false;
    }

    /// <summary>
    /// index가 유효한 값인지 판별한다.
    /// </summary>
    /// <returns></returns>
    public bool IsValidInHeroListByIndex(int index)
    {
        if (index < 0 || index >= m_HeroData.heroList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// PartyList에서 hero가 몇 번째에 존재하는지 확인한다.
    /// </summary>
    public int GetIndexOfHeroInParty(UnitData data)
    {
        return Utility.FindIndexOf(m_HeroData.partyList, data);
    }

    public int GetIndexOfHeroInList(UnitData data)
    {
        return Utility.FindIndexOf(m_HeroData.heroList, data);
    }

    public UnitData GetHumalDataByIndex(int id)
    {
        foreach (var hero in m_HeroData.heroList)
        {
            if (hero.ID == id)
                return hero;
        }

        throw new Exception("ID : " + id + "는(은) 존재하지 않는 ID입니다.");
    }

    public UnitData GetDataByOrder(string key, int index)
    {
        if (!IsValidInHeroListByIndex(index))
            throw new Exception("유효하지 않은 Index 값입니다.");

        if (key == "Next")
        {
            if (!IsValidInHeroListByIndex(index + 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index + 1);
            return m_HeroData.heroList[index + 1];
        }
        else if (key == "Previous")
        {
            if (!IsValidInHeroListByIndex(index - 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index - 1);
            return m_HeroData.heroList[index - 1];
        }
        else
        {
            throw new Exception("유효하지 않은 Key 값입니다.");
        }
    }

    public void SwapPartyData(int from, int to)
    {
        Utility.SwapListElement(m_HeroData.partyList, from, to);
    }

    public void UpdatePartyLeader()
    {
        for (int i = 0; i < m_HeroData.partyList.Count; i++)
        {
            if (i == 0)
            {
                m_HeroData.partyList[i].SetLeader(true);
                //m_HeroData.partyList[i].IsLeader = true;
                LeaderHero = m_HeroData.partyList[i];
            }
            else
            {
                m_HeroData.partyList[i].SetLeader(false);
                //m_HeroData.partyList[i].IsLeader = false;
            }
        }
    }
    #endregion

    #region Currency
    private void InitializedVirtualCurrencyDic()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(
            request,
            OnInitializedVirtualCurrencyDicSuccess,
            //(result) => amount = result.VirtualCurrency[type.ToString()],
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    private void OnInitializedVirtualCurrencyDicSuccess(GetUserInventoryResult result)
    {
        VirtualCurrencyDic = result.VirtualCurrency;

        foreach (ECurrencyType type in Enum.GetValues(typeof(ECurrencyType)))
        {
            if (VirtualCurrencyDic.ContainsKey(type.ToString()))
                uiMgr.InvokeCurrencyUI(type, VirtualCurrencyDic[type.ToString()]);
        }
    }

    public int GetCurrency(ECurrencyType type)
    {
        if (!Social.localUser.authenticated)
            return -1;

        if (VirtualCurrencyDic.ContainsKey(type.ToString()))
            return VirtualCurrencyDic[type.ToString()];

        Debug.LogError("do not exist " + type.ToString());
        return -1;
    }

    private void AddCurrency(ECurrencyType currencyTag, int amount)
    {
        if (!Social.localUser.authenticated)
            return;

        var request = new AddUserVirtualCurrencyRequest() { VirtualCurrency = currencyTag.ToString(), Amount = amount };
        PlayFabClientAPI.AddUserVirtualCurrency(
            request,
            (result) => uiMgr.InvokeCurrencyUI(currencyTag, result.Balance),
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    private void SubstractCurrency(ECurrencyType currencyTag, int amount)
    {
        var request = new SubtractUserVirtualCurrencyRequest() { VirtualCurrency = currencyTag.ToString(), Amount = amount };
        PlayFabClientAPI.SubtractUserVirtualCurrency(
            request,
            (result) => uiMgr.InvokeCurrencyUI(currencyTag, result.Balance),
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    public void GetGold()
    {
        try
        {
            SetCurrencyAmount(ECurrencyType.GD, 1000);
            //GameData.currencyList[(int)EGoodsType.Gold].count += 1000;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            //PopUpManager.Instance.PopUp(ex.ToString(), EPopUpType.Caution);
        }
    }

    public void SetCurrencyAmount(ECurrencyType currencyType, int num)
    {
        if (!Social.localUser.authenticated)
            return;

        if (GetCurrency(currencyType) < 0)
            throw new Exception(currencyType.ToString() + " is empty");

        var amount = GetCurrency(currencyType) + num;
        if (amount > int.MaxValue)
        {
            PopUpManager.Instance.PopUp(currencyType.ToString() + " 재화가 한계치입니다!", EPopUpType.Caution);
            throw new Exception("Overflow Max Goods Value");
        }
        else if (amount < 0)
        {
            PopUpManager.Instance.PopUp(currencyType.ToString() + " 재화가 부족합니다!", EPopUpType.Caution);
            throw new Exception("Insufficient Goods Value");
        }
        else
        {
            if (num > 0)
                AddCurrency(currencyType, num);
            else
                SubstractCurrency(currencyType, num);
        }
        // AddElementInCurrencyList((int)currencyType, num);
        //m_GameData.CurrencyList[(int)EGoodsType].count += num;
    }
    #endregion

    #region Inventory
    public void GetInventory()
    {
        if (!Social.localUser.authenticated)
            return;

        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(
            request,
            OnGetInventorySuccess,
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    private void OnGetInventorySuccess(GetUserInventoryResult result)
    {
        for (int i = 0; i < Tags.CurrencyTags.Length; i++)
            Debug.Log("현재 통화 " + Tags.CurrencyTags[i] + " : " + result.VirtualCurrency[Tags.CurrencyTags[i]]);

        for (int i = 0; i < result.Inventory.Count; i++)
        {
            var inven = result.Inventory[i];
            Debug.Log(inven.DisplayName + " / " + inven.UnitCurrency + " / " + inven.UnitPrice + " / " + inven.ItemInstanceId + " / " + inven.RemainingUses);
        }
    }

    public void PurchaseItem(EItemCatalog catalog, string itemId, string tag, int price)
    {
        if (!Social.localUser.authenticated)
            return;

        var request = new PurchaseItemRequest()
        {
            CatalogVersion = catalog.ToString(),
            ItemId = itemId,
            VirtualCurrency = tag,
            Price = price
        };

        PlayFabClientAPI.PurchaseItem(
            request,
            (result) => Debug.Log("아이템 구입 성공"),
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    public void ConsumeItem(int consumeCount, string itemId)
    {
        if (!Social.localUser.authenticated)
            return;

        var request = new ConsumeItemRequest()
        {
            ConsumeCount = consumeCount,
            ItemInstanceId = itemId
        };

        PlayFabClientAPI.ConsumeItem(
            request,
            (result) => Debug.Log("아이템 사용 성공"),
            (error) => Debug.Log("아이템 사용 실패")
        );
    }
    #endregion

    #region Load & Save
    public void InitializedData()
    {
        InitializedGameData();
        InitializedHeroData();

        SaveData();
    }

    public IEnumerator LoadDataCo()
    {
        yield return StartCoroutine(LoadHeroDataCo());
        yield return StartCoroutine(LoadGameDataCo());
    }

    private void InitializedHeroData()
    {
        for (int i = 0; i < HeroData.HeroMaxSize; i++)
            m_HeroData.heroUnlockList[i] = false;

        m_HeroData.heroDic.Clear();
        m_HeroData.heroList.Clear();
        m_HeroData.partyList.Clear();

        m_HeroData.heroUnlockList[0] = true;

        if (m_HeroData.originHeroDataList.Count != 0)
            AddNewHumal(0);

        m_HeroData.isLoadComplete = true;
    }

    private void InitializedGameData()
    {
        m_GameData.inventoryDic.Clear();
        foreach (var itemData in ItemDataList)
        {
            switch (itemData.ItemType)
            {
                case EItemType.None:
                    break;

                case EItemType.Consume:
                    ConsumeItem item = new ConsumeItem((ConsumeItemData)itemData, 1);
                    m_GameData.inventoryDic.Add(itemData.Name, item);
                    break;

                case EItemType.Equipment:
                    break;
            }
        }

        for (int i = 0; i < m_GameData.facilGold.Length; i++)
            m_GameData.facilGold[i] = 0;

        m_GameData.sfx = 1f;
        m_GameData.bgm = 1f;

        m_GameData.isNew = true;

        m_GameData.lastLogInTime = DateTime.Now;
        m_GameData.lastLogInTimeStr = m_GameData.lastLogInTime.ToString();

        m_GameData.isLoadComplete = true;
    }

    private void CalculateSaveTime()
    {
        string offTime = m_GameData.lastLogInTimeStr;
        DateTime exitTime = Convert.ToDateTime(offTime);

        DateTime dateTime = DateTime.Now;
        TimeSpan timeStamp = dateTime - exitTime;

        int timeCalDay = timeStamp.Days;
        if (timeCalDay > 0)
            InitializedTodayBuyingLimit();
    }

    private void InitializedTodayBuyingLimit()
    {
        foreach (var item in m_GameData.inventoryDic.Values)
        {
            if (item as CountableItem != null)
                ((CountableItem)item).SetTodayBuyingAmount(0);
        }
    }

    private void SetupHero()
    {
        try
        {
            if (m_HeroData.heroList.Count != 0)
            {
                for (int i = 0; i < m_HeroData.heroList.Count; i++)
                {
                    m_HeroData.heroList[i].sprite = m_HeroData.heroSpriteList[m_HeroData.heroList[i].ID];
                    m_HeroData.heroList[i].animCtrl = m_HeroData.heroAnimCtrlList[m_HeroData.heroList[i].ID];
                }

                for (int i = 0; i < m_HeroData.partyList.Count; i++)
                {
                    m_HeroData.partyList[i].sprite = m_HeroData.heroSpriteList[m_HeroData.partyList[i].ID];
                    m_HeroData.partyList[i].animCtrl = m_HeroData.heroAnimCtrlList[m_HeroData.partyList[i].ID];
                }

                for (int i = 0; i < m_HeroData.heroList.Count; i++)
                {
                    var targetName = m_HeroData.heroList[i].ID;
                    if (!m_HeroData.heroDic.ContainsKey(targetName))
                        m_HeroData.heroDic.Add(targetName, m_HeroData.heroList[i]);
                }

                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    for (int i = 0; i < m_HeroData.heroList.Count; i++)
                    {
                        int index = i;
                        if (lobbyHeroList.Contains(m_HeroData.heroList[index]))
                            continue;

                        Addressables.InstantiateAsync(
                            lobbyHeroReference,
                            Vector3.zero,
                            Quaternion.identity
                        ).Completed += (handle) =>
                        {
                            if (handle.Result.TryGetComponent(out LobbyHero hero))
                            {
                                var heroStat = m_HeroData.heroList[index];
                                hero.UnitSetup(heroStat);
                                lobbyHeroList.Add(m_HeroData.heroList[index]);
                            }
                        };

                        /*var hero = Instantiate(lobbyHeroPrefab, Vector3.zero,
                            Quaternion.identity).GetComponent<LobbyHero>();*/

                        //heroStat.mySprite = HeroData.heroSpriteList[heroStat.Data.ID];
                        //heroStat.animCtrl = HeroData.heroAnimCtrlList[heroStat.Data.ID];
                    }
                }
            }
            else
            {
                throw new Exception("m_HeroData.heroList is empty!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            popUpMgr.PopUp(ex.Message, EPopUpType.Warning);
        }
    }

    public IEnumerator LoadHeroDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            bool isContainsData = false;

            foreach (var eachData in result.Data)
            {
                if (eachData.Key.Contains("HeroData"))
                {
                    isContainsData = true;
                    m_HeroData = JsonUtility.FromJson<HeroData>(eachData.Value.Value);

                    SetupHero();

                    ConstructHeroDic();
                    ConstructHumalPieceAmountDic();

                    m_HeroData.isLoadComplete = true;
                    Debug.LogWarning("Load Hero Data");
                }
            }

            if(!isContainsData)
                InitializedHeroData();

        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator LoadGameDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            InitializedVirtualCurrencyDic();

            bool isContainsData = false;

            foreach (var eachData in result.Data)
            {
                if (eachData.Key.Contains("GameData"))
                {
                    isContainsData = true;
                    m_GameData = JsonUtility.FromJson<GameData>(eachData.Value.Value);

                    CalculateSaveTime();

                    m_GameData.isLoadComplete = true;
                }
            }

            if (!isContainsData)
                InitializedGameData();

        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }

    public void SaveData()
    {
        if (!Social.localUser.authenticated)
            return;

        ResettingHeroList();
        ResettingHumalPieceAmountList();

        m_GameData.isLoadComplete = false;
        m_HeroData.isLoadComplete = false;

        Dictionary<string, string> dataDic = new Dictionary<string, string>();
        dataDic.Add("GameData", JsonUtility.ToJson(m_GameData));
        dataDic.Add("HeroData", JsonUtility.ToJson(m_HeroData));
        SetData(dataDic);
    }

    private void SetData(Dictionary<string, string> dataDic)
    {
        var request = new UpdateUserDataRequest() { Data = dataDic, Permission = UserDataPermission.Public };

        try
        {
            PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("Update Data!");
            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError("Error : " + e.Message);
        }
    }

    private void SaveTime()
    {
        m_GameData.lastLogInTime = DateTime.Now;
        m_GameData.lastLogInTimeStr = m_GameData.lastLogInTime.ToString();
    }
    #endregion

    public int GetHumalPieceAmount(string key)
    {
        if (m_HeroData.humalPieceAmountDic.ContainsKey(key))
            return m_HeroData.humalPieceAmountDic[key];
        else
            return -1;
    }

    public void AddNewHumal(int id)
    {
        var unitData = m_HeroData.originHeroDataList[id];
        
        if (unitData == null)
            throw new Exception("해당 index의 unitData가 없습니다.");

        if(IsContainsInHumalList(id))
        {
            Debug.Log("이미 영웅이 있어서 파편으로 변환!");
            AddHumalPiece(unitData.KoName, 100);
        }
        else
        {
            popUpMgr.PopUp("휴멀 - " + unitData.KoName + " 뽑기 성공!", EPopUpType.Notice);

            m_HeroData.heroList.Add(new UnitData(unitData));
            m_HeroData.heroUnlockList[id] = true;

            AddHumalPiece(unitData.KoName, 0);

            SetupHero();
            uiMgr.UpdateHeroPanel();
        }
    }

    public void AddHumalPiece(string key, int amount)
    {
        popUpMgr.PopUp("휴멀 [" + key + "] 파편 " + amount +"개 획득!", EPopUpType.Notice);
        if (m_HeroData.humalPieceAmountDic.ContainsKey(key))
            m_HeroData.humalPieceAmountDic[key] += amount;
        else
            m_HeroData.humalPieceAmountDic.Add(key, amount);
    }

    public void SubtractHumalPiece(string key, int amount)
    {
        if (m_HeroData.humalPieceAmountDic.ContainsKey(key))
            m_HeroData.humalPieceAmountDic[key] -= amount;
    }

    private void DisplayPlayfabError(PlayFabError error)
    {
        Debug.LogError("error : " + error.GenerateErrorReport());
    }

    public void AddInventoryItem(Item item, int amount)
    {
        if (m_GameData.inventoryDic.TryGetValue(item.Data.Name, out Item currentItem))
        {
            if (currentItem as ConsumeItem != null)
            {
                ((ConsumeItem)currentItem).AddAmount(amount);
                ((ConsumeItem)currentItem).AddTodayBuyingAmount(1);
            }
        }
        else
        {
            m_GameData.inventoryDic.Add(item.Data.Name, item);
            if (item as ConsumeItem != null)
            {
                ((ConsumeItem)item).SetAmount(amount);
                ((ConsumeItem)item).SetTodayBuyingAmount(1);
            }
        }
    }

    public bool GetItemByKey(string key, out Item outItem)
    {
        foreach (var item in m_GameData.inventoryDic.Values)
        {
            if (item.Data.Name == key)
            {
                outItem = item;
                return true;
            }
        }

        outItem = null;
        return false;
    }

    public ItemData GetItemDataByKey(string key)
    {
        return mItemDataList.Find(x => x.Name == key);
    }

    public int GetTodayBuyingAmountOfItemByKey(string key)
    {
        if (m_GameData.inventoryDic.TryGetValue(key, out Item item))
        {
            if (item as CountableItem != null)
                return ((CountableItem)item).TodayBuyingAmount;
        }

        throw new Exception("does not exist Key");
    }

    private void OnApplicationQuit()
    {
        if (Social.localUser.authenticated)
        {
            SaveTime();
            SaveData();
        }
    }
}