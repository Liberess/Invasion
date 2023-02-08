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
using Cysharp.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    private UIManager uiMgr;
    private PopUpManager popUpMgr;

    [Header("==== Hero Object Prefab ====")]
    [SerializeField] private AssetReference heroReference;
    [SerializeField] private AssetReference enemyReference;
    [SerializeField] private AssetReference lobbyHeroReference;

    private List<UnitData> lobbyHeroList = new List<UnitData>();
    //[SerializeField] private GameObject heroPrefab;
    //[SerializeField] private GameObject lobbyHeroPrefab;

    public UnitData LeaderHero { get; private set; }

    #region 인스턴스화

    public static DataManager Instance { get; private set; }

    [Header("==== Game Data Information ===="), Space(10)] [SerializeField]
    private GameData m_GameData = new GameData();

    public GameData GameData
    {
        get => m_GameData;
    }

    [Header("==== Hero Data Information ===="), Space(10)] [SerializeField]
    private HumalData m_HumalData = new HumalData();

    public HumalData HumalData
    {
        get => m_HumalData;
    }

    [Header("==== Item Data Information ===="), Space(10)] [SerializeField]
    private List<ItemData> mItemDataList = new List<ItemData>();

    public List<ItemData> ItemDataList => mItemDataList;

    #endregion

    private Dictionary<string, int> VirtualCurrencyDic = new Dictionary<string, int>();

    public List<Sprite> goodsSpriteList = new List<Sprite>();

    [SerializeField] private Text isDownTxt;
    [SerializeField] private Text isProgressTxt;
    private bool isDownStart = false;

    [SerializeField, Range(5f, 60f)] private readonly float saveDataInterval = 30f;
    private DateTime lastSaveTime;
    private AsyncOperationHandle updateBundleHandle;

    public Action OnSaveDataAction;

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

        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += () => AutoSave().Forget();

        OnSaveDataAction += SaveData;

        GameManager.Instance.OnApplicationStart();

        StartCoroutine(UpdateResources());
    }

    public void SetStageInfo(StageInfo info) => GameData.stageInfo = info;

    #region Update Resources

    public IEnumerator UpdateResources()
    {
        UpdateUnitPrefab();
        UpdateHumalAssets();
        UpdateGoodsSprite();
        UpdateHeroAnimCtrl();
        UpdateHeroCardIcon();
        UpdateItemData();
        UpdateOriginDB();
        UpdateHumalPickDB();
        UpdateEnemyAsset();

        yield return null;
    }

    private void UpdateUnitPrefab()
    {
        Addressables.LoadAssetAsync<GameObject>(heroReference).Completed +=
            handle => m_GameData.unitPrefabAry[(int)EUnitQueueType.Hero] = handle.Result;
        
        Addressables.LoadAssetAsync<GameObject>(enemyReference).Completed +=
            handle => m_GameData.unitPrefabAry[(int)EUnitQueueType.Enemy] = handle.Result;

        Addressables.LoadAssetAsync<GameObject>(lobbyHeroReference).Completed +=
            handle => m_GameData.lobbyHumalPrefab = handle.Result;
    }

    private void UpdateHumalPickDB()
    {
        Addressables.LoadAssetAsync<HumalPickDB>(Tags.HumalPickDBLabel).Completed +=
        handle =>
        {
            foreach (var db in handle.Result.Entities)
            {
                if (!m_HumalData.humalPickDBList.Contains(db))
                    m_HumalData.humalPickDBList.Add(db);
            }

            m_HumalData.humalPickDBList = m_HumalData.humalPickDBList.OrderBy(x => x.id).ToList();
        };
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
        Debug.Log("UpdateOriginDB");
        updateBundleHandle = Addressables.LoadAssetsAsync<UnitOriginDB>(
            Tags.UnitOriginDBLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "유닛 DB 다운로드 시작";
                    Debug.Log("유닛 DB 다운로드 시작");
                    m_HumalData.originHumalDataList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                foreach(var unitDB in result.HumalDBList)
                {
                    if (!m_HumalData.originHumalDataList.Contains(unitDB))
                        m_HumalData.originHumalDataList.Add(unitDB);
                }

                m_HumalData.originHumalDataList = m_HumalData.originHumalDataList.OrderBy(x => x.ID).ToList();

                foreach(var unitDB in result.EnemyDBList)
                {
                    if(!m_GameData.enemyDataList.Contains(unitDB))
                        m_GameData.enemyDataList.Add(unitDB);
                }
            }
        );
    }

    private void UpdateHumalAssets()
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

    private void UpdateEnemyAsset()
    {
        updateBundleHandle = Addressables.LoadAssetsAsync<Sprite>(
            Tags.EnemyLabel,
            (result) =>
            {
                if (!isDownStart)
                {
                    isDownStart = true;
                    isDownTxt.text = "몬스터 스프라이트 다운로드 시작";
                    Debug.Log("몬스터 스프라이트 다운로드 시작");
                    m_GameData.enemySpriteList.Clear();
                    StartCoroutine(UpdateBundleProgressTxtCo());
                }

                if (!m_GameData.enemySpriteList.Contains(result))
                    m_GameData.enemySpriteList.Add(result);
            }
        );
        
        updateBundleHandle = Addressables.LoadAssetsAsync<RuntimeAnimatorController>(
            Tags.EnemyAnimCtrlLabel,
            (result) =>
            {
                if(!m_GameData.enemyAnimCtrlList.Contains(result))
                    m_GameData.enemyAnimCtrlList.Add(result);
            }
        );

        updateBundleHandle.Completed += (AsyncOperationHandle handle) =>
        {
            updateBundleHandle = handle;
            isDownTxt.text = "다운로드 완료";
            isProgressTxt.text = "100%";
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
    private void ConstructDataDic()
    {
        m_HumalData.humalDic.Clear();

        //Json에 저장된 humalList안의 내용들을 humalDic에 저장
        for (int i = 0; i < m_HumalData.humalList.Count; i++)
        {
            int id = m_HumalData.humalList[i].ID;
            if (!m_HumalData.humalDic.ContainsKey(id))
                m_HumalData.humalDic.Add(id, m_HumalData.humalList[i]);
        }

        for (int i = 0; i < m_HumalData.partyList.Count; i++)
        {
            int id = m_HumalData.partyList[i].ID;
            if (!m_HumalData.humalDic.ContainsKey(id))
                m_HumalData.humalDic.Add(id, m_HumalData.partyList[i]);
        }

        m_HumalData.humalSpriteDic.Clear();
        for (int i = 0; i < m_HumalData.humalSpriteList.Count; i++)
            m_HumalData.humalSpriteDic.Add(i, m_HumalData.humalSpriteList[i]);

        SetupHero();
    }

    /// <summary>
    /// humalList에 id가 존재하는지 확인한다.
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
    /// partyList에 id가 존재하는지 확인한다.
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
    public bool IsValidInPartyListByIndex(int index)
    {
        if (index < 0 || index >= m_HumalData.partyList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// index가 유효한 값인지 판별한다.
    /// </summary>
    /// <returns></returns>
    public bool IsValidInHumalListByIndex(int index)
    {
        if (index < 0 || index >= m_HumalData.humalList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// partyList에서 hero가 몇 번째에 존재하는지 확인한다.
    /// </summary>
    public int GetIndexOfHumalInParty(UnitData data)
    {
        return m_HumalData.partyList.FindIndex(e => e.ID == data.ID);
        //return Utility.FindIndexOf(m_HumalData.partyList, data);
    }

    public int GetIndexOfHumalInList(UnitData data)
    {
        return m_HumalData.humalList.FindIndex(e => e.ID == data.ID);
        //return Utility.FindIndexOf(m_HumalData.humalList, data);
    }

    public UnitData GetHumalDataByID(int id)
    {
        foreach (var hero in m_HumalData.humalDic.Values)
        {
            if (hero.ID == id)
                return hero;
        }

        throw new Exception("ID : " + id + "는(은) 존재하지 않는 ID입니다.");
    }

    public UnitData GetDataByOrder(string key, int index)
    {
        if (!IsValidInHumalListByIndex(index))
            throw new Exception("유효하지 않은 Index 값입니다.");

        if (key == "Next")
        {
            if (!IsValidInHumalListByIndex(index + 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index + 1);
            return m_HumalData.humalList[index + 1];
        }
        else if (key == "Previous")
        {
            if (!IsValidInHumalListByIndex(index - 1))
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
            (result) =>
            {
                VirtualCurrencyDic[currencyTag.ToString()] = result.Balance;
                uiMgr.InvokeCurrencyUI(currencyTag, result.Balance);
                SaveData();
            },
            PlayFabManager.Instance.PlayFabErrorDebugLog
        );
    }

    private void SubstractCurrency(ECurrencyType currencyTag, int amount)
    {
        if (!Social.localUser.authenticated)
            return;

        var request = new SubtractUserVirtualCurrencyRequest() { VirtualCurrency = currencyTag.ToString(), Amount = amount };
        PlayFabClientAPI.SubtractUserVirtualCurrency(
            request,
            (result) =>
            {
                VirtualCurrencyDic[currencyTag.ToString()] = result.Balance;
                uiMgr.InvokeCurrencyUI(currencyTag, result.Balance);
                SaveData();
            },
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
        m_HumalData.isLoadComplete = false;

        m_HumalData.humalDic.Clear();
        m_HumalData.humalList.Clear();
        m_HumalData.partyList.Clear();

        m_HumalData.humalPieceAmountList.Clear();
        if (m_HumalData.originHumalDataList.Count != 0)
        {
            foreach(var data in m_HumalData.originHumalDataList)
            {
                data.SetUnlock(false);
                data.SetParty(false);
                data.SetLeader(false);
                m_HumalData.humalPieceAmountList.Add(new HumalPiece(data.ID, data.KoName, 0));
            }

            AddNewHumal(0);
        }

        uiMgr.InitHeroPanel();
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
/*            if (m_HumalData.humalList.Count > 0)
            {
                for (int i = 0; i < m_HumalData.humalList.Count; i++)
                {
                    int targetID = m_HumalData.humalList[i].ID;

                    m_HumalData.humalList[i].sprite = m_HumalData.humalSpriteList[targetID];
                    m_HumalData.humalList[i].animCtrl = m_HumalData.humalAnimCtrlList[targetID];

                    if (!m_HumalData.humalDic.ContainsKey(targetID))
                        m_HumalData.humalDic.Add(targetID, m_HumalData.humalList[i]);
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
                    }
                }
            }
            else if(m_HumalData.partyList.Count > 0)
            {
                for (int i = 0; i < m_HumalData.partyList.Count; i++)
                {
                    int targetID = m_HumalData.partyList[i].ID;

                    m_HumalData.partyList[i].sprite = m_HumalData.humalSpriteList[targetID];
                    m_HumalData.partyList[i].animCtrl = m_HumalData.humalAnimCtrlList[targetID];

                    if (!m_HumalData.humalDic.ContainsKey(targetID))
                        m_HumalData.humalDic.Add(targetID, m_HumalData.partyList[i]);
                }

                if (SceneManager.GetActiveScene().name == "Lobby")
                {
                    for (int i = 0; i < m_HumalData.partyList.Count; i++)
                    {
                        int index = i;
                        if (lobbyHeroList.Contains(m_HumalData.partyList[index]))
                            continue;

                        Addressables.InstantiateAsync(
                            lobbyHeroReference,
                            Vector3.zero,
                            Quaternion.identity
                        ).Completed += (handle) =>
                        {
                            if (handle.Result.TryGetComponent(out LobbyHero hero))
                            {
                                var heroStat = m_HumalData.partyList[index];
                                hero.UnitSetup(heroStat);
                                lobbyHeroList.Add(m_HumalData.partyList[index]);
                            }
                        };
                    }
                }
            }
            else
            {
                throw new Exception("humalList or partyList empty!");
            }*/
            if(m_HumalData.humalDic.Count > 0)
            {
                bool isLobby = SceneManager.GetActiveScene().name == "Lobby";

                foreach (var data in m_HumalData.humalDic.Values)
                {
                    int targetID = data.ID;

                    data.sprite = m_HumalData.humalSpriteList[targetID];
                    data.animCtrl = m_HumalData.GetHumalAnimCtrl(targetID);

                    if (isLobby)
                    {
                        if (lobbyHeroList.Contains(data))
                            continue;

                        var lobbyHero = Instantiate(m_GameData.lobbyHumalPrefab, Vector3.zero, Quaternion.identity).GetComponent<LobbyHero>();
                        var heroStat = data;
                        lobbyHero.UnitSetup(heroStat);
                        lobbyHeroList.Add(data);

                        /*Addressables.InstantiateAsync(
                            lobbyHeroReference, Vector3.zero, Quaternion.identity
                        ).Completed += (handle) =>
                        {
                            if (handle.Result.TryGetComponent(out LobbyHero hero))
                            {
                                Debug.Log("awdwadawd");
                                var heroStat = data;
                                hero.UnitSetup(heroStat);
                                lobbyHeroList.Add(data);
                            }
                        };*/
                    }
                }
            }
            else
            {
                throw new Exception("humalDic is empty!");
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
            if(result.Data.ContainsKey(Tags.HumalDataTag))
            {
                m_HumalData = new HumalData();
                m_HumalData = JsonUtility.FromJson<HumalData>(result.Data[Tags.HumalDataTag].Value);

                ConstructDataDic();

                //SetupHero();

                uiMgr.InitHeroPanel();

                m_HumalData.isLoadComplete = true;
            }
            else
            {
                InitializedHumalData();
            }

        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator LoadGameDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            InitializedVirtualCurrencyDic();

            if (result.Data.ContainsKey(Tags.GameDataTag))
            {
                m_GameData = new GameData();
                m_GameData = JsonUtility.FromJson<GameData>(result.Data[Tags.GameDataTag].Value);

                CalculateSaveTime();

                m_GameData.isLoadComplete = true;
            }
            else
            {
                InitializedGameData();
            }

        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }

    private async UniTaskVoid AutoSave()
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(saveDataInterval), DelayType.UnscaledDeltaTime);
            SaveData();
        }
    }

    public void SaveData()
    {
        if (!Social.localUser.authenticated)
            return;

        TimeSpan timeCal = DateTime.Now - lastSaveTime;
        if(timeCal.Seconds < saveDataInterval)
            return;
        
        lastSaveTime = DateTime.Now;
        SaveTime();

        Dictionary<string, string> dataDic = new Dictionary<string, string>
        {
            { Tags.HumalDataTag, JsonUtility.ToJson(m_HumalData) },
            { Tags.GameDataTag, JsonUtility.ToJson(m_GameData) }
        };

        SetData(dataDic);
    }

    private void SetData(Dictionary<string, string> dataDic)
    {
        var request = new UpdateUserDataRequest()
        { 
            Data = dataDic,
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
        {
            Debug.Log("Set Data Complete!");
        }, DisplayPlayfabError);
    }

    private void SaveTime()
    {
        m_GameData.lastLogInTime = DateTime.Now;
        m_GameData.lastLogInTimeStr = m_GameData.lastLogInTime.ToString();
    }
    #endregion

    public void AddNewHumal(int id)
    {
        try
        {
            if(id < 0 || id >= m_HumalData.originHumalDataList.Count)
                throw new Exception("해당 id-" + id + "의 unitData가 없습니다.");

            var unitData = m_HumalData.originHumalDataList[id];

            if (IsContainsInHumalList(id))
            {
                Debug.Log("이미 영웅이 있어서 파편으로 변환!");
                AddHumalPiece(id, 100);
            }
            else
            {
                UnitData newData = new UnitData(unitData);

                popUpMgr.PopUp("휴멀 - " + newData.KoName + " 뽑기 성공!", EPopUpType.Notice);

                if (!m_HumalData.humalDic.ContainsKey(newData.ID))
                    m_HumalData.humalDic.Add(newData.ID, newData);

                if (m_HumalData.humalSpriteDic.ContainsKey(id))
                {
                    newData.sprite = m_HumalData.humalSpriteDic[id];
                    unitData.sprite = newData.sprite;
                }

                newData.animCtrl = m_HumalData.GetHumalAnimCtrl(id);
                unitData.animCtrl = newData.animCtrl;

                newData.SetUnlock(true);

                if (m_HumalData.partyList.Count <= 0)
                {
                    newData.SetParty(true);
                    newData.SetLeader(true);
                    m_HumalData.partyList.Add(newData);
                    //uiMgr.SwapSlotToParty(id);
                    //uiMgr.SetToPartyList(id);
                }
                else
                {
                    newData.SetParty(false);
                    newData.SetLeader(false);
                    m_HumalData.humalList.Add(newData);
                }

                uiMgr.UpdateHumalSlotDataByID(id);
                if(!m_HumalData.isLoadComplete)
                    uiMgr.SetEnabledHumalSlotByID(id);

                SetupHero();
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public bool TryGetHumalPieceAmount(int id, out int amount)
    {
        try
        {
            HumalPiece humalPiece = m_HumalData.humalPieceAmountList.Find(x => x.id == id);
            if(humalPiece != null)
            {
                amount = humalPiece.amount;
                return true;
            }
            else
            {
                amount = -1;
                return false;
            }
        }
        catch(Exception ex) 
        {
            Debug.LogError(ex.ToString());
            amount = -1;
            return false;
        }
    }

    private bool TryGetHumalPiece(int id, out HumalPiece humalPiece)
    {
        humalPiece = m_HumalData.humalPieceAmountList.Find(x => x.id == id);
        if (humalPiece != null)
            return true;
        else
            return false;
    }

    public void AddHumalPiece(int id, int amount)
    {
        if(TryGetHumalPiece(id, out HumalPiece humalPiece))
        {
            humalPiece.amount += amount;
            uiMgr.UpdateHumalSlotByID(id);

            popUpMgr.PopUp("휴멀 [" + humalPiece.name + "] 파편 " + amount + "개 획득!", EPopUpType.Notice);
        }
    }

    public void SubtractHumalPiece(int id, int amount)
    {
        if (TryGetHumalPiece(id, out HumalPiece humalPiece))
        {
            humalPiece.amount -= amount;
            uiMgr.UpdateHumalSlotByID(id);
        }
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

    public async UniTaskVoid LoadScene(string sceneName)
    {
        PlayFabManager.Instance.ClearLogInSuccessAction();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != "Lobby")
            return;

        StartCoroutine(OnSceneLoadedCo());
    }

    private IEnumerator OnSceneLoadedCo()
    {
        lobbyHeroList.Clear();
        SetupHero();

        yield return new WaitForEndOfFrame();

        uiMgr = UIManager.Instance;
        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += uiMgr.InitHeroPanel;
        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += () => uiMgr.UpdateCurrencyUI(0f).Forget();
        PlayFabManager.Instance.InvokeLogInSuccessAction();
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