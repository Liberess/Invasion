using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
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
    private List<UnitData> lobbyHeroList = new List<UnitData>();
    //[SerializeField] private GameObject heroPrefab;
    //[SerializeField] private GameObject lobbyHeroPrefab;

    public UnitData LeaderHero { get; private set; }

    #region 인스턴스화
    public static DataManager Instance { get; private set; }

    [Header("==== Game Data Information ===="), Space(10)]
    [SerializeField] private GameData m_GameData;
    public GameData GameData { get => m_GameData; }

    [Header("==== Hero Data Information ===="), Space(10)]
    [SerializeField] private HumalData m_HumalData;
    public HumalData HumalData { get => m_HumalData; }

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

    [SerializeField] private Text isDownTxt;
    [SerializeField] private Text isProgressTxt;
    private bool isDownStart = false;

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
    }

    private void Start()
    {
        uiMgr = UIManager.Instance;
        popUpMgr = PopUpManager.Instance;

        GameManager.Instance.OnApplicationStart();

#if UNITY_EDITOR
        StartCoroutine(UpdateResources());
#endif
    }

    public void SetStageInfo(StageInfo info) => GameData.stageInfo = info;

    #region Update Resources
    public IEnumerator UpdateResources()
    {
        UpdateGoodsSprite();
        UpdateHeroSprite();
        UpdateOriginDB();
        UpdateHeroAnimCtrl();
        UpdateHeroCardIcon();
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
        updateBundleHandle = Addressables.LoadAssetsAsync<UnitOriginDB>(
            Tags.UnitOriginDBLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "유닛 DB 다운로드 시작";
                    Debug.Log("유닛 DB 다운로드 시작");
                    HumalData.originHumalDataList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                foreach(var unitDB in result.HumalDBList)
                {
                    if (!m_HumalData.originHumalDataList.Contains(unitDB))
                        m_HumalData.originHumalDataList.Add(unitDB);
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
                    Debug.Log("몬스터 스프라이트 다운로드 시작");
                    m_EnemySpriteList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if (!m_EnemySpriteList.Contains(result))
                    m_EnemySpriteList.Add(result);
            }
        );

        updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
        {
            updateBundleHandle = handle;
            isDownTxt.text = "다운로드 완료";
            isProgressTxt.text = "100%";
        };
    }

    private void UpdateHeroSprite()
    {
        updateBundleHandle = Addressables.LoadAssetsAsync<Sprite>(
            Tags.HumalSpriteLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "휴멀 스프라이트 다운로드 시작";
                    Debug.Log("휴멀 스프라이트 다운로드 시작");
                    m_HumalData.humalSpriteDic.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if(int.TryParse(result.name[..result.name.IndexOf('_')], out int spriteID))
                {
                    if (!m_HumalData.humalSpriteDic.ContainsKey(spriteID))
                        m_HumalData.humalSpriteDic.Add(spriteID, result);
                }
            }
        );

        updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
        {
            updateBundleHandle = handle;
            isDownTxt.text = "다운로드 완료";
            isProgressTxt.text = "100%";

            m_HumalData.humalSpriteList.Clear();
            foreach (var sprite in m_HumalData.humalSpriteDic.Values)
            {
                if(!m_HumalData.humalSpriteList.Contains(sprite))
                    m_HumalData.humalSpriteList.Add(sprite);
            }
        };
    }

    private void UpdateHeroCardIcon()
    {
        updateBundleHandle = Addressables.LoadAssetsAsync<Sprite>(
            Tags.HumalIconLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "휴멀 아이콘 다운로드 시작";
                    Debug.Log("휴멀 아이콘 다운로드 시작");
                    m_HumalData.humalCardIconList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if (!m_HumalData.humalCardIconList.Contains(result))
                    m_HumalData.humalCardIconList.Add(result);
            }
        );

        updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
        {
            updateBundleHandle = handle;
            isDownTxt.text = "다운로드 완료";
            isProgressTxt.text = "100%";
        };
    }

    //[ContextMenu("Update Hero Anim Controller")]
    private void UpdateHeroAnimCtrl()
    {
        updateBundleHandle = Addressables.LoadAssetsAsync<RuntimeAnimatorController>(
            Tags.HumalAnimCtrlLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "휴멀 AnimCtrl 다운로드 시작";
                    Debug.Log("휴멀 AnimCtrl 다운로드 시작");
                    m_HumalData.humalAnimCtrlList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if (!m_HumalData.humalAnimCtrlList.Contains(result))
                    m_HumalData.humalAnimCtrlList.Add(result);
            }
        );

        updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
        {
            updateBundleHandle = handle;
            isDownTxt.text = "다운로드 완료";
            isProgressTxt.text = "100%";
        };
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
        m_HumalData.humalDic = new Dictionary<int, UnitData>();
        m_HumalData.humalDic.Clear();

        //Json에 저장된 humalList안의 내용들을 humalDic에 저장
        for (int i = 0; i < m_HumalData.humalList.Count; i++)
            m_HumalData.humalDic.Add(m_HumalData.humalList[i].ID, m_HumalData.humalList[i]);
    }

    private void ResettingHeroList()
    {
        m_HumalData.humalList.Clear();
        m_HumalData.humalList = new List<UnitData>(m_HumalData.humalDic.Values);
    }

    private void ConstructHumalPieceAmountDic()
    {
        //m_HumalData.humalPieceAmountDic = new Dictionary<string, int>();
        m_HumalData.humalPieceAmountDic = new HumalPieceDictionary();

        for (int i = 0; i < m_HumalData.humalPieceAmountList.Count; i++)
        {
            m_HumalData.humalPieceAmountDic.Add(
                m_HumalData.humalPieceAmountList[i].name,
                m_HumalData.humalPieceAmountList[i].amount
            );
        }
    }

    private void ResettingHumalPieceAmountList()
    {
        m_HumalData.humalPieceAmountList.Clear();

        List<HumalPiece> temList = new List<HumalPiece>();
        foreach(var data in m_HumalData.humalPieceAmountDic)
            temList.Add(new HumalPiece(data.Key, data.Value));

        m_HumalData.humalPieceAmountList = temList;
    }

    /// <summary>
    /// HumalList에 id가 존재하는지 확인한다.
    /// </summary>
    public bool IsContainsInHumalList(int id)
    {
        foreach (var heroData in m_HumalData.humalList)
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
        foreach (var HumalData in m_HumalData.partyList)
        {
            if (HumalData.ID == id)
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
        if (index < 0 || index >= m_HumalData.humalList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// PartyList에서 hero가 몇 번째에 존재하는지 확인한다.
    /// </summary>
    public int GetIndexOfHeroInParty(UnitData data)
    {
        return Utility.FindIndexOf(m_HumalData.partyList, data);
    }

    public int GetIndexOfHeroInList(UnitData data)
    {
        return Utility.FindIndexOf(m_HumalData.humalList, data);
    }

    public UnitData GetHumalDataByIndex(int id)
    {
        foreach (var hero in m_HumalData.humalList)
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
            return m_HumalData.humalList[index + 1];
        }
        else if (key == "Previous")
        {
            if (!IsValidInHeroListByIndex(index - 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index - 1);
            return m_HumalData.humalList[index - 1];
        }
        else
        {
            throw new Exception("유효하지 않은 Key 값입니다.");
        }
    }

    public void SwapPartyData(int from, int to)
    {
        Utility.SwapListElement(m_HumalData.partyList, from, to);
    }

    public void UpdatePartyLeader()
    {
        for (int i = 0; i < m_HumalData.partyList.Count; i++)
        {
            if (i == 0)
            {
                m_HumalData.partyList[i].SetLeader(true);
                //m_HumalData.partyList[i].IsLeader = true;
                LeaderHero = m_HumalData.partyList[i];
            }
            else
            {
                m_HumalData.partyList[i].SetLeader(false);
                //m_HumalData.partyList[i].IsLeader = false;
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
        if (!Social.localUser.authenticated)
            return;

        Debug.Log("SubstractCurrency : " + currencyTag.ToString() + "/" + amount);
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

    public bool SetCurrencyAmount(ECurrencyType currencyType, int num)
    {
        if (!Social.localUser.authenticated)
            return false;

        if (GetCurrency(currencyType) < 0)
        {
            popUpMgr.PopUp(currencyType.ToString() + " is empty", EPopUpType.Caution);
            return false;
        }

        var amount = GetCurrency(currencyType) + num;
        if (amount > int.MaxValue)
        {
            popUpMgr.PopUp(currencyType.ToString() + " 재화가 한계치입니다!", EPopUpType.Caution);
            //throw new Exception("Overflow Max Goods Value");
            return false;
        }
        else if (amount < 0)
        {
            popUpMgr.PopUp(currencyType.ToString() + " 재화가 부족합니다!", EPopUpType.Caution);
            //throw new Exception("Insufficient Goods Value");
            return false;
        }
        else
        {
            if (num > 0)
                AddCurrency(currencyType, Mathf.Abs(num));
            else
                SubstractCurrency(currencyType, Mathf.Abs(num));

            return true;
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
        InitializedHumalData();

        SaveData();
    }

    public IEnumerator LoadDataCo()
    {
        yield return StartCoroutine(LoadHumalDataCo());
        yield return StartCoroutine(LoadGameDataCo());
    }

    private void InitializedHumalData()
    {
        for (int i = 0; i < HumalData.HumalMaxSize; i++)
            m_HumalData.humalUnlockList[i] = false;

        m_HumalData.humalDic.Clear();
        m_HumalData.humalList.Clear();
        m_HumalData.partyList.Clear();

        m_HumalData.humalUnlockList[0] = true;

        if (m_HumalData.originHumalDataList.Count != 0)
            AddNewHumal(0);

        m_HumalData.isLoadComplete = true;
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
            if (m_HumalData.humalList.Count != 0)
            {
                for (int i = 0; i < m_HumalData.humalList.Count; i++)
                {
                    m_HumalData.humalList[i].sprite = m_HumalData.humalSpriteList[m_HumalData.humalList[i].ID];
                    m_HumalData.humalList[i].animCtrl = m_HumalData.humalAnimCtrlList[m_HumalData.humalList[i].ID];
                }

                for (int i = 0; i < m_HumalData.partyList.Count; i++)
                {
                    m_HumalData.partyList[i].sprite = m_HumalData.humalSpriteList[m_HumalData.partyList[i].ID];
                    m_HumalData.partyList[i].animCtrl = m_HumalData.humalAnimCtrlList[m_HumalData.partyList[i].ID];
                }

                for (int i = 0; i < m_HumalData.humalList.Count; i++)
                {
                    var targetName = m_HumalData.humalList[i].ID;
                    if (!m_HumalData.humalDic.ContainsKey(targetName))
                        m_HumalData.humalDic.Add(targetName, m_HumalData.humalList[i]);
                }

                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    for (int i = 0; i < m_HumalData.humalList.Count; i++)
                    {
                        int index = i;
                        if (lobbyHeroList.Contains(m_HumalData.humalList[index]))
                            continue;

                        Addressables.InstantiateAsync(
                            lobbyHeroReference,
                            Vector3.zero,
                            Quaternion.identity
                        ).Completed += (handle) =>
                        {
                            if (handle.Result.TryGetComponent(out LobbyHero hero))
                            {
                                var heroStat = m_HumalData.humalList[index];
                                hero.UnitSetup(heroStat);
                                lobbyHeroList.Add(m_HumalData.humalList[index]);
                            }
                        };

                        /*var hero = Instantiate(lobbyHeroPrefab, Vector3.zero,
                            Quaternion.identity).GetComponent<LobbyHero>();*/

                        //heroStat.mySprite = HumalData.humalSpriteList[heroStat.Data.ID];
                        //heroStat.animCtrl = HumalData.humalAnimCtrlList[heroStat.Data.ID];
                    }
                }
            }
            else
            {
                throw new Exception("m_HumalData.humalList is empty!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            popUpMgr.PopUp(ex.Message, EPopUpType.Warning);
        }
    }

    public IEnumerator LoadHumalDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            bool isContainsData = false;

            foreach (var eachData in result.Data)
            {
                if (eachData.Key.Contains("HumalData"))
                {
                    isContainsData = true;
                    m_HumalData = JsonUtility.FromJson<HumalData>(eachData.Value.Value);

                    SetupHero();

                    ConstructHeroDic();
                    ConstructHumalPieceAmountDic();

                    m_HumalData.isLoadComplete = true;
                }
            }

            if(!isContainsData)
                InitializedHumalData();

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
        m_HumalData.isLoadComplete = false;

        Dictionary<string, string> dataDic = new Dictionary<string, string>();
        dataDic.Add("GameData", JsonUtility.ToJson(m_GameData));
        dataDic.Add("HumalData", JsonUtility.ToJson(m_HumalData));
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
        if (m_HumalData.humalPieceAmountDic.ContainsKey(key))
            return m_HumalData.humalPieceAmountDic[key];
        else
            return -1;
    }

    public void AddNewHumal(int id)
    {
        var unitData = m_HumalData.originHumalDataList[id];
        
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

            m_HumalData.humalList.Add(new UnitData(unitData));
            unitData.sprite = m_HumalData.humalSpriteList[id];
            unitData.animCtrl = m_HumalData.humalAnimCtrlList[id];
            m_HumalData.humalUnlockList[id] = true;

            AddHumalPiece(unitData.KoName, 0);

            SetupHero();
            uiMgr.UpdateHeroPanel();
        }
    }

    public void AddHumalPiece(string key, int amount)
    {
        popUpMgr.PopUp("휴멀 [" + key + "] 파편 " + amount +"개 획득!", EPopUpType.Notice);
        if (m_HumalData.humalPieceAmountDic.ContainsKey(key))
            m_HumalData.humalPieceAmountDic[key] += amount;
        else
            m_HumalData.humalPieceAmountDic.Add(key, amount);
    }

    public void SubtractHumalPiece(string key, int amount)
    {
        if (m_HumalData.humalPieceAmountDic.ContainsKey(key))
            m_HumalData.humalPieceAmountDic[key] -= amount;
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