using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private enum HeroSortType { Menu = -1, Level, Grade, DPS }
    private enum SortingType { Ascending, Descending }
    private enum StatusType { HP, Critical, AP, Dodge, DP, Cost }

    public static UIManager Instance { get; private set; }

    private DataManager dataMgr;
    private UnityMainThreadDispatcher dispatcher;

    [Header("==== Game UI ====")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject offNotice;
    public bool IsPanel { get; private set; }

    [Header("==== Hero UI ===="), Space(10)]
    [SerializeField] private GameObject heroPartyGrid;
    [SerializeField] private GameObject heroSlotGrid;
    [SerializeField] private GameObject heroMenuPanel;
    public GameObject HeroMenuPanel => heroMenuPanel;
    public Button partyMenuBtn;
    [HideInInspector] public Text partyMenuTxt;
    [SerializeField] private HeroDetailInfoPanel heroDetailInfoPanel;
    [SerializeField] private Button nextHeroBtn;
    [SerializeField] private Button previousHeroBtn;
    private int currentHeroIndex;
    private int currentPartyIndex;
    private bool isPartyDetailInfo = false;
    private HeroSlot currentHeroSlot;

    [Header("==== Sort UI ===="), Space(10)]
    [SerializeField] private Button sortBtn;
    [SerializeField] private Text sortTxt;
    [SerializeField] private Button asSortBtn;
    [SerializeField] private Button desSortBtn;
    [SerializeField] private GameObject sortPanel;
    [SerializeField] private List<Button> sortBtnList = new List<Button>();
    private SortingType sortingType;
    private HeroSortType heroSortingType;

    [Header("==== HeroSlot Object Pooling ===="), Space(10)]
    [SerializeField] private GameObject heroSlotPrefab;
    private int heroSlotIndex = 0;
    [SerializeField] private int heroSlotMaxCount = 20;
    [SerializeField] private List<HeroSlot> heroSlotList = new List<HeroSlot>();
    private List<HeroSlot> partySlotList = new List<HeroSlot>();
    private List<HeroSlot> tempHeroSlotList = new List<HeroSlot>();

    [Header("==== Map UI ===="), Space(10)]
    [SerializeField] private StageInfoPanel stageReadyPanel;
    private List<Button> mapBtnList = new List<Button>();
    private List<GameObject> detailStagePanelList = new List<GameObject>();
    private Dictionary<int, List<Button>> detailStageBtnDic = new Dictionary<int, List<Button>>();

    public UnityAction HidePanelAction;
    public UnityAction HideHeroInfoPanelAction;
    //public Action<int> UpdateCurrencyUIAction;
    public List<Action<int>> UpdateCurrencyUIActionList = new List<Action<int>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        UpdateCurrencyUIActionList.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(ECurrencyType)).Length; i++)
            UpdateCurrencyUIActionList.Add(null);
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;

        if (UnityMainThreadDispatcher.Exists())
            dispatcher = UnityMainThreadDispatcher.Instance();

        if (heroDetailInfoPanel == null)
            heroDetailInfoPanel = FindObjectOfType<HeroDetailInfoPanel>();

        HideHeroInfoPanelAction += () => heroMenuPanel.SetActive(false);

        SetupMapUI();

        InitHeroSlotObjectPool();
        InitSortButton();

        partyMenuTxt = partyMenuBtn.GetComponentInChildren<Text>();
        partyMenuBtn.onClick.AddListener(SetPartyToHumalSlot);

        //StartCoroutine(InitHeroPanelCoru());
        //StartCoroutine(UpdateGoodsUICo(0.1f));

        //PlayFabManager.Instance.OnPlayFabLoginSuccessAction += () => StartCoroutine(UpdateCurrencyUICo());
    }

    private IEnumerator UpdateCurrencyUICo()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (ECurrencyType type in Enum.GetValues(typeof(ECurrencyType)))
            InvokeCurrencyUI(type, dataMgr.GetCurrency(type));
    }

    public void InvokeCurrencyUI(ECurrencyType type, int amount)
    {
        UpdateCurrencyUIActionList[(int)type]?.Invoke(amount);
    }

    public void SetActivePauseUI()
    {
        if (!optionPanel.activeSelf)
        {
            optionPanel.SetActive(true);
            SoundManager.Instance.PlaySFX("Pause In");

            if (IsPanel)
                HidePanelAction();
        }
        else
        {
            optionPanel.SetActive(false);
            SoundManager.Instance.PlaySFX("Pause Out");
        }
    }

    #region Offline Reward
    public void SetActiveOfflineRewardNotice(bool active)
    {
        if(active)
            offNotice.gameObject.GetComponent<Animator>().SetTrigger("doShow");
        else
            offNotice.gameObject.GetComponent<Animator>().SetTrigger("doHide");
    }

    public void SetOfflineRewardUI(string infoTxt, string numTxt)
    {
        offNotice.transform.Find("OffTxt").gameObject.GetComponent<Text>().text = infoTxt;
        offNotice.transform.Find("JelatinNumTxt").gameObject.GetComponent<Text>().text = numTxt;
    }
    #endregion

    #region Hero Panel
    private void InitSortButton()
    {
        sortBtn.onClick.AddListener(() => OnClickSortButton(HeroSortType.Menu));

        asSortBtn.onClick.AddListener(() => sortingType = SortingType.Ascending);
        asSortBtn.onClick.AddListener(() => OnClickSortButton(heroSortingType));

        desSortBtn.onClick.AddListener(() => sortingType = SortingType.Descending);
        desSortBtn.onClick.AddListener(() => OnClickSortButton(heroSortingType));

        for (int i = 0; i < sortBtnList.Count; i++)
        {
            int temp = i;
            sortBtnList[i].onClick.AddListener(() => OnClickSortButton((HeroSortType)temp));
            sortBtnList[i].onClick.AddListener(() => heroSortingType = (HeroSortType)temp);
        }
    }

    public void InitHeroPanel() => StartCoroutine(InitHeroPanelCoru());

    private IEnumerator InitHeroPanelCoru()
    {
        InitPartySlot();
        InitHeroSlot();

        yield return null;
    }

    private void InitPartySlot()
    {
        for (int i = 0; i < dataMgr.HumalData.partyList.Count; i++)
        {
            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetParent(heroPartyGrid.transform);
            heroSlot.transform.localScale = new Vector3(1f, 1f, 1f);
            heroSlot.HumalDataSetup(dataMgr.HumalData.partyList[i]);
            heroSlot.SetEnabledHumalSlot(true);
            partySlotList.Add(heroSlot);
        }
    }

    private void InitHeroSlot()
    {
        List<int> PartyIDList = new List<int>();
        foreach (var hero in dataMgr.HumalData.partyList)
            PartyIDList.Add(hero.ID);

        heroSlotList.Clear();
        for (int i = 0; i < dataMgr.HumalData.humalList.Count; i++)
        {
            int id = dataMgr.HumalData.humalList[i].ID;
            if (PartyIDList.Contains(id))
                continue;
            else if (IsContainsHeroSlotList(id))
                continue;

            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetAsLastSibling();
            heroSlot.HumalDataSetup(dataMgr.HumalData.humalList[i]);
            heroSlot.SetEnabledHumalSlot(true);
            heroSlot.SetEnabledDraggable(false);
            heroSlotList.Add(heroSlot);
        }

        for (int i = 0; i < dataMgr.HumalData.originHumalDataList.Count; i++)
        {
            int id = dataMgr.HumalData.originHumalDataList[i].ID;
            if (PartyIDList.Contains(id))
                continue;
            else if (IsContainsHeroSlotList(id))
                continue;

            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetAsLastSibling();
            heroSlot.HumalDataSetup(dataMgr.HumalData.originHumalDataList[i]);
            heroSlot.SetEnabledHumalSlot(false);
            heroSlot.SetEnabledDraggable(false);
            heroSlotList.Add(heroSlot);
        }

        OnClickSortButton(HeroSortType.Level);
    }

    public void SetEnabledHumalSlotByID(int id)
    {
        HeroSlot heroSlot = heroSlotList.Find(x => x.HumalData.ID == id);
        if (heroSlot != null)
        {
            heroSlot.SetEnabledHumalSlot(true);
            OnClickSortButton(heroSortingType);
        }
    }

    public void UpdateHumalSlotDataByID(int id)
    {
        HeroSlot heroSlot = heroSlotList.Find(x => x.HumalData.ID == id);
        if (heroSlot != null)
        {
            if(dataMgr.HumalData.humalDic.TryGetValue(id, out UnitData data))
                heroSlot.UpdateHumalData(data);
        }
    }

    public void UpdateHumalSlotByID(int id)
    {
        HeroSlot heroSlot = heroSlotList.Find(x => x.HumalData.ID == id);
        if (heroSlot != null)
            heroSlot.UpdateSlot();
    }

    private bool IsContainsHeroSlotList(int id)
    {
        foreach(var heroSlot in heroSlotList)
        {
            if(heroSlot.HumalData.ID == id) 
                return true;
        }

        return false;
    }

    public void SetCurrentHumalSlot(HeroSlot heroSlot) => currentHeroSlot = heroSlot;

    public void SetPartyToHumalSlot()
    {
        int id = currentHeroSlot.HumalData.ID;
        
        if (currentHeroSlot.HumalData.IsParty)
        {
            if (dataMgr.HumalData.humalDic.ContainsKey(id))
            {
                dataMgr.HumalData.humalDic[id].SetParty(false);
                dataMgr.HumalData.humalDic[id].SetLeader(false);
                dataMgr.HumalData.humalList.Add(currentHeroSlot.HumalData);
                dataMgr.HumalData.partyList.Remove(currentHeroSlot.HumalData);
            }
            
            currentHeroSlot.SetEnabledDraggable(false);
            heroSlotList.Add(currentHeroSlot);
            partySlotList.Remove(currentHeroSlot);
        }
        else
        {
            if (dataMgr.HumalData.humalDic.ContainsKey(id))
            {
                if(dataMgr.HumalData.partyList.Count <= 0)
                    dataMgr.HumalData.humalDic[id].SetLeader(true);
                    
                dataMgr.HumalData.humalDic[id].SetParty(true);
                dataMgr.HumalData.humalList.Remove(currentHeroSlot.HumalData);
                dataMgr.HumalData.partyList.Add(currentHeroSlot.HumalData);
            }
            
            heroSlotList.Remove(currentHeroSlot);
            partySlotList.Add(currentHeroSlot);
            
            currentHeroSlot.SetEnabledDraggable(true);
            currentHeroSlot.transform.SetParent(null);
            currentHeroSlot.transform.SetParent(heroPartyGrid.transform);
        }
        
        heroMenuPanel.SetActive(false);
        OnClickSortButton(heroSortingType);
    }
    
    private void OnClickSortButton(HeroSortType sortType)
    {
        switch (sortType)
        {
            case HeroSortType.Menu:
                if (sortPanel.activeSelf)
                    sortPanel.SetActive(false);
                else
                    sortPanel.SetActive(true);
                break;

            case HeroSortType.Level:
                sortTxt.text = "레벨";
                if (sortingType == SortingType.Ascending)
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenBy(x => x.HumalData.Level).ToList();
                }
                else
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenByDescending(x => x.HumalData.Level).ToList();
                }
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                sortPanel.SetActive(false);
                break;

            case HeroSortType.Grade:
                sortTxt.text = "등급";
                if (sortingType == SortingType.Ascending)
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenBy(x => x.HumalData.Level).ToList();
                }
                else
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenByDescending(x => x.HumalData.Level).ToList();

                }
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                sortPanel.SetActive(false);
                break;

            case HeroSortType.DPS:
                sortTxt.text = "전투력";
                if (sortingType == SortingType.Ascending)
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenBy(x => x.HumalData.DPS).ToList();
                }
                else
                {
                    heroSlotList = heroSlotList.
                        OrderByDescending(x => x.HumalData.IsUnlock == true).
                        ThenByDescending(x => x.HumalData.DPS).ToList();
                }
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                sortPanel.SetActive(false);
                break;

            default:
                break;
        }
    }
    #endregion

    #region Map Panel
    private void InitMapButton()
    {
        GameObject mapPanel = GameObject.Find("MapCanvas").transform.Find("MapPanel").gameObject;

        // Set Map Button List
        GameObject btnParent = mapPanel.transform.
            Find("Scroll View").Find("Viewport").gameObject;

        mapBtnList.Clear();
        for (int i = 0; i < btnParent.transform.childCount; i++)
        {
            int temp = i;
            var child = btnParent.transform.GetChild(i).GetComponent<Button>();
            child.name = "Stage" + (i + 1) + "Panel";
            child.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Stage " + (i + 1);
            child.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "골목길";
            child.onClick.AddListener(() => DetailStagePanelCtrl(temp));
            mapBtnList.Add(child);
        }
    }

    private void InitDetailStagePanel()
    {
        GameObject mapPanel = GameObject.Find("MapCanvas").transform.Find("MapPanel").gameObject;

        // Set Detail Stage Panel
        GameObject panelParent = mapPanel.transform.
            Find("DetailStagePanelGroup").gameObject;

        detailStagePanelList.Clear();
        for (int i = 0; i < panelParent.transform.childCount; i++)
        {
            var child = panelParent.transform.GetChild(i).gameObject;
            child.name = "DetailStage" + (i + 1) + "Panel";
            detailStagePanelList.Add(child);
        }
    }

    private void InitDetailButton()
    {
        Debug.Log("수정 필요!! StageInfo");

        // Set Detail Button List
        detailStageBtnDic.Clear();
        for (int i = 0; i < detailStagePanelList.Count; i++)
        {
            detailStageBtnDic.Add(i, new List<Button>());
            for (int j = 0; j < detailStagePanelList[i].transform.childCount; j++)
            {
                Button btn = detailStagePanelList[i].transform.GetChild(j).
                    gameObject.GetComponent<Button>();

                if (btn != null && btn.name != "QuitBtn")
                {
                    string text = (i + 1) + "-" + (j + 1);
                    btn.name = text + "Btn";

                    if (btn.GetComponentInChildren<Text>() != null)
                        btn.GetComponentInChildren<Text>().text = text;

                    btn.onClick.AddListener(() => stageReadyPanel.gameObject.SetActive(true));
                    btn.onClick.AddListener(() => stageReadyPanel.SetupStageInfo(
                        new StageInfo("테헤12란로", text, EStageLevel.Easy, 0, 1)));
                    detailStageBtnDic[i].Add(btn);
                }
            }
        }
    }

    private void SetupMapUI()
    {
        InitMapButton();
        InitDetailStagePanel();
        InitDetailButton();
    }

    private void DetailStagePanelCtrl(int index)
    {
        if (detailStagePanelList[index].activeSelf)
            detailStagePanelList[index].SetActive(false);
        else
            detailStagePanelList[index].SetActive(true);
    }
    #endregion

    #region Hero Slot Object Pooling
    private void InitHeroSlotObjectPool()
    {
        tempHeroSlotList.Clear();

        for (int i = 0; i < heroSlotMaxCount; i++)
            tempHeroSlotList.Add(CreateNewObj(i));
    }

    private HeroSlot CreateNewObj(int index = 0)
    {
        var newObj = Instantiate(heroSlotPrefab, transform.position,
            Quaternion.identity).GetComponent<HeroSlot>();
        newObj.name = "HeroSlot " + index;
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static HeroSlot GetObj()
    {
        if (Instance.tempHeroSlotList[Instance.heroSlotIndex].gameObject.activeSelf)
            return null;

        if (Instance.tempHeroSlotList.Count > 0)
        {
            var obj = Instance.tempHeroSlotList[Instance.heroSlotIndex];
            obj.transform.SetParent(Instance.heroSlotGrid.transform);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.gameObject.SetActive(true);
            ++Instance.heroSlotIndex;

            if (Instance.heroSlotIndex >= Instance.heroSlotMaxCount)
                Instance.heroSlotIndex = 0;

            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObj();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnObj(HeroSlot obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);

        --Instance.heroSlotIndex;

        if (Instance.heroSlotIndex < 0)
            Instance.heroSlotIndex = Instance.heroSlotMaxCount;
    }
    #endregion

    #region Hero Detail Info Panel
    public void SetHeroIndex(int value)
    {
        if (value < 0 || value >= dataMgr.HumalData.humalList.Count)
            return;

        currentHeroIndex = value;
        UpdateHeroOrderBtn();
    }

    /// <summary>
    /// HeroDetailInfoPanel의 내용을 Update한다.
    /// </summary>
    /// <param name="id"> 세부 정보 패널에 표시할 영웅의 번호 </param>
    public void UpdateHeroDetailInfo(UnitData data)
    {
        try
        {
            if (data == null)
                throw new Exception("해당 데이터가 없습니다.");

            int index = -1;
            if (data.IsParty)
                index = dataMgr.GetIndexOfHumalInParty(data);
            else
                index = dataMgr.GetIndexOfHumalInList(data);

            if (index < 0)
                throw new Exception(Time.time + " 유효하지 않은 index 값입니다.");

            isPartyDetailInfo = data.IsParty;
            currentPartyIndex = index;
            currentHeroIndex = index;

            UpdateHeroOrderBtn();
            dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(data));
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void UpdateHeroOrderBtn()
    {
        if (isPartyDetailInfo)
        {
            if (dataMgr.IsValidInPartyListByIndex(currentPartyIndex + 1))
                nextHeroBtn.interactable = true;
            else
                nextHeroBtn.interactable = false;

            if (dataMgr.IsValidInPartyListByIndex(currentPartyIndex - 1))
                previousHeroBtn.interactable = true;
            else
                previousHeroBtn.interactable = false;
        }
        else
        {
            if (dataMgr.IsValidInHumalListByIndex(currentHeroIndex + 1))
                nextHeroBtn.interactable = true;
            else
                nextHeroBtn.interactable = false;

            if (dataMgr.IsValidInHumalListByIndex(currentHeroIndex - 1))
                previousHeroBtn.interactable = true;
            else
                previousHeroBtn.interactable = false;
        }
    }

    public void OnClickNextHero()
    {
        if(isPartyDetailInfo) 
        {
            dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
                dataMgr.GetDataByOrder("Next", currentPartyIndex)));
        }
        else
        {
            dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
                dataMgr.GetDataByOrder("Next", currentHeroIndex)));
        }
    }

    public void OnClickPreviousHero()
    {
        if (isPartyDetailInfo)
        {
            dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
                dataMgr.GetDataByOrder("Previous", currentPartyIndex)));
        }
        else
        {
            dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
                dataMgr.GetDataByOrder("Previous", currentHeroIndex)));
        }
    }
    #endregion
}