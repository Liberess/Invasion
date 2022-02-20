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

    private enum SortingType
    {
        Ascending = 0, //오름차
        Descending      //내림차
    }

    public static UIManager Instance { get; private set; }

    private DataManager dataMgr;

    [Header("==== Hero UI ====")]
    [SerializeField] private GameObject heroPartyGrid;
    [SerializeField] private GameObject heroSlotGrid;

    [Header("==== Sort UI ===="), Space(10)]
    [SerializeField] private Button sortBtn;
    [SerializeField] private Button asSortBtn;
    [SerializeField] private Button desSortBtn;
    [SerializeField] private GameObject sortPanel;
    [SerializeField] private List<Button> sortBtnList = new List<Button>();
    [SerializeField] private SortingType sortingType;
    [SerializeField] private HeroSortType heroSortingType;

    [Header("==== HeroSlot Object Pooling ===="), Space(10)]
    [SerializeField] private GameObject heroSlotPrefab;
    [SerializeField] private int heroSlotIndex = 0;
    [SerializeField] private int heroSlotMaxCount = 20;
    [SerializeField] private List<HeroSlot> heroSlotList = new List<HeroSlot>();
    [SerializeField] private List<HeroSlot> partySlotList = new List<HeroSlot>();
    [SerializeField] private List<HeroSlot> tempHeroSlotList = new List<HeroSlot>();

    [Header("==== Map UI ===="), Space(10)]
    [SerializeField] private StageInfoPanel stageReadyPanel;
    private List<Button> mapBtnList = new List<Button>();
    private List<GameObject> detailStagePanelList = new List<GameObject>();
    private Dictionary<int, List<Button>> detailStageBtnDic = new Dictionary<int, List<Button>>();

    public UnityAction HidePanelAction;
    public UnityAction HideHeroInfoPanelAction;

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

        SetupMapUI();

        InitHeroSlotObjectPool();
        InitSortButton();
        InitPartySlot();
        InitHeroSlot();
    }

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

    private void InitPartySlot()
    {
        for (int i = 0; i < dataMgr.heroData.partyList.Count; i++)
        {
            HeroSlot heroSlot = GetObj();
            heroSlot.transform.SetParent(heroPartyGrid.transform);
            heroSlot.transform.localScale = new Vector3(1f, 1f, 1f);
            heroSlot.UnitSetup(dataMgr.heroData.heroList[i]);
            partySlotList.Add(heroSlot);
        }
    }

    private void InitHeroSlot()
    {
        List<int> IDList = new List<int>();

        foreach (var hero in dataMgr.heroData.partyList)
            IDList.Add(hero.ID);

        for (int i = 0; i < dataMgr.heroData.heroList.Count; i++)
        {
            if (IDList.Contains(dataMgr.heroData.heroList[i].ID))
                continue;

            HeroSlot heroSlot = GetObj();
            heroSlot.UnitSetup(dataMgr.heroData.heroList[i]);
            heroSlotList.Add(heroSlot);
        }
    }

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
                        new StageInfo("테헤란로", text, StageLevel.Easy, 0, 1)));
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
                    heroSlotList = heroSlotList.OrderBy(x => x.MyStatus.level).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.MyStatus.level).ToList();
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                break;

            case HeroSortType.Grade:
                Debug.Log("OnClickSortBtn : Grade");
                if (sortingType == SortingType.Ascending)
                    heroSlotList = heroSlotList.OrderBy(x => x.MyStatus.level).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.MyStatus.level).ToList();
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                break;

            case HeroSortType.DPS:
                if (sortingType == SortingType.Ascending)
                    heroSlotList = heroSlotList.OrderBy(x => x.MyStatus.DPS).ToList();
                else
                    heroSlotList = heroSlotList.OrderByDescending(x => x.MyStatus.DPS).ToList();
                foreach (var slot in heroSlotList)
                {
                    slot.transform.SetParent(null);
                    slot.transform.SetParent(heroSlotGrid.transform);
                }
                break;

            default:
                break;
        }
    }

    #region Object Pooling
    private void InitHeroSlotObjectPool()
    {
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

    public void UpdateHeroSlotUI()
    {
        for (int i = 0; i < dataMgr.heroData.heroList.Count; i++)
        {
            if (dataMgr.heroData.partyList.Contains(dataMgr.heroData.heroList[i]))
            {
                Debug.Log("파티에 추가되어있음");
                continue;
            }
        }
    }
}