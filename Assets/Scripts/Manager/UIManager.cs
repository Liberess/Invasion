using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private enum HeroSortType
    {
        Menu = -1,
        Level,
        Grade,
        DPS
    }
    //오름차                내림차
    private enum SortingType { Ascending = 0, Descending }
    private enum StatusType { HP = 0, Critical, AP, Dodge, DP, Cost }

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
    [SerializeField] private HeroDetailInfoPanel heroDetailInfoPanel;
    [SerializeField] private GameObject nextHeroBtn;
    [SerializeField] private GameObject previousHeroBtn;
    private int currentHeroIndex;

    [Header("==== Sort UI ===="), Space(10)]
    [SerializeField] private Button sortBtn;
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

        //StartCoroutine(InitHeroPanelCoru());
        //StartCoroutine(UpdateGoodsUICo(0.1f));

        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += () => StartCoroutine(InitHeroPanelCoru());
        //PlayFabManager.Instance.OnPlayFabLoginSuccessAction += () => StartCoroutine(UpdateCurrencyUICo());
    }

    private IEnumerator UpdateCurrencyUICo()
    {
        yield return new WaitForSeconds(1.0f);

        Debug.Log("UpdateCurrencyUICo : " + Time.time);

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

    public void InitHeroPanel()
    {
        StartCoroutine(InitHeroPanelCoru());
    }

    private IEnumerator InitHeroPanelCoru()
    {
        yield return StartCoroutine(DataManager.Instance.UpdateResources());

        InitPartySlot();
        InitHeroSlot();
    }

    private void InitPartySlot()
    {
        partySlotList.Clear();

        for (int i = 0; i < dataMgr.HeroData.partyList.Count; i++)
        {
            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetParent(heroPartyGrid.transform);
            heroSlot.transform.localScale = new Vector3(1f, 1f, 1f);
            heroSlot.HeroStatusSetup(dataMgr.HeroData.partyList[i]);
            partySlotList.Add(heroSlot);
        }
    }

    private void InitHeroSlot()
    {
        heroSlotList.Clear();

        Debug.Log("InitHeroSlot");

        List<int> PartyIDList = new List<int>();
        foreach (var hero in dataMgr.HeroData.partyList)
            PartyIDList.Add(hero.Data.ID);

        Debug.Log("count : " + dataMgr.HeroData.heroList.Count);
        for (int i = 0; i < dataMgr.HeroData.heroList.Count; i++)
        {
            int id = dataMgr.HeroData.heroList[i].Data.ID;
            Debug.Log("id : " + id);
            if (PartyIDList.Contains(id))
                continue;
            else if (IsContainsHeroSlotList(id))
            {
                Debug.Log(3);
                continue;
            }

            HeroSlot heroSlot = GetObj();
            heroSlot.HeroStatusSetup(dataMgr.HeroData.heroList[i]);
            heroSlotList.Add(heroSlot);
                Debug.Log(4);
        }
    }

    public void UpdateHeroPanel()
    {
        Debug.Log("UpdateHeroPanel");
        for (int i = 0; i < dataMgr.HeroData.partyList.Count; i++)
        {
            int id = dataMgr.HeroData.partyList[i].Data.ID;
            if (partySlotList.Find(x => x.HeroStatus.Data.ID == id) != null)
                continue;

            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetParent(heroPartyGrid.transform);
            heroSlot.transform.localScale = new Vector3(1f, 1f, 1f);
            heroSlot.HeroStatusSetup(dataMgr.HeroData.partyList[i]);
            partySlotList.Add(heroSlot);
        }

        List<int> PartyIDList = new List<int>();
        foreach (var hero in partySlotList)
            PartyIDList.Add(hero.HeroStatus.Data.ID);

        Debug.Log("count2 : " + dataMgr.HeroData.heroList.Count);
        for (int i = 0; i < dataMgr.HeroData.heroList.Count; i++)
        {
            int id = dataMgr.HeroData.heroList[i].Data.ID;
            Debug.Log("id2 : " + id);
            if (PartyIDList.Contains(id))
                continue;
            else if (IsContainsHeroSlotList(id))
            {

            Debug.Log(33);
                continue;
            }

            HeroSlot heroSlot = GetObj();
            heroSlot.HeroStatusSetup(dataMgr.HeroData.heroList[i]);
            heroSlotList.Add(heroSlot);
            Debug.Log(44);
        }
    }

    private bool IsContainsHeroSlotList(int id)
    {
        foreach(var heroSlot in heroSlotList)
        {
            if(heroSlot.HeroStatus.Data.ID == id) 
                return true;
        }

        return false;
    }

    /// <summary>
    /// 영웅 슬롯에서 파티 슬롯으로 스왑했을 때 사용한다.
    /// </summary>
    /// <param name="id"> Hero의 Data.ID </param>
    public void SwapSlotToParty(int id)
    {
        int index = 0;

        for (index = 0; index < heroSlotList.Count; index++)
        {
            if (heroSlotList[index].HeroStatus.Data.ID == id)
                break;
        }

        partySlotList.Add(heroSlotList[index]);
        heroSlotList.RemoveAt(index);
    }

    /// <summary>
    /// 파티 슬롯에서 영웅 슬롯으로 스왑했을 때 사용한다.
    /// </summary>
    /// <param name="id"> Hero의 ID </param>
    public void SwapPartyToSlot(int id)
    {
        int index = 0;

        for (index = 0; index < partySlotList.Count; index++)
        {
            if (partySlotList[index].HeroStatus.Data.ID == id)
                break;
        }

        heroSlotList.Add(partySlotList[index]);
        partySlotList.RemoveAt(index);
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
                if (sortingType == SortingType.Ascending)
                    heroSlotList = heroSlotList.OrderBy(x => x.HeroStatus.Data.Level).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.HeroStatus.Data.Level).ToList();
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                sortPanel.SetActive(false);
                break;

            case HeroSortType.Grade:
                if (sortingType == SortingType.Ascending)
                    heroSlotList = heroSlotList.OrderBy(x => x.HeroStatus.Data.Level).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.HeroStatus.Data.Level).ToList();
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                sortPanel.SetActive(false);
                break;

            case HeroSortType.DPS:
                if (sortingType == SortingType.Ascending)
                    heroSlotList = heroSlotList.OrderBy(x => x.HeroStatus.DPS).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.HeroStatus.DPS).ToList();
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
        if (value < 0 || value >= dataMgr.HeroData.heroList.Count)
            return;

        currentHeroIndex = value;
        UpdateHeroOrderBtn();
    }

    /// <summary>
    /// HeroDetailInfoPanel의 내용을 Update한다.
    /// </summary>
    /// <param name="ID"> 세부 정보 패널에 표시할 영웅의 번호 </param>
    public void UpdateHeroDetailInfo(int ID)
    {
        if (heroDetailInfoPanel == null)
            return;

        currentHeroIndex = dataMgr.GetIndexOfHeroInList(dataMgr.GetHumalDataByIndex(ID));
        UpdateHeroOrderBtn();

        dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
            dataMgr.GetHumalDataByIndex(ID)));
    }

    private void UpdateHeroOrderBtn()
    {
        if (dataMgr.IsValidInHeroListByIndex(currentHeroIndex + 1))
            nextHeroBtn.GetComponent<Button>().interactable = true;
        else
            nextHeroBtn.GetComponent<Button>().interactable = false;

        if (dataMgr.IsValidInHeroListByIndex(currentHeroIndex - 1))
            previousHeroBtn.GetComponent<Button>().interactable = true;
        else
            previousHeroBtn.GetComponent<Button>().interactable = false;
    }

    public void OnClickNextHero()
    {
        dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
            dataMgr.GetDataByOrder("Next", currentHeroIndex)));
    }

    public void OnClickPreviousHero()
    {
        dispatcher.Enqueue(() => heroDetailInfoPanel.UpdateHeroInfo(
            dataMgr.GetDataByOrder("Previous", currentHeroIndex)));
    }
    #endregion
}