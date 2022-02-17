﻿using System;
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
    [SerializeField] private List<HeroSlot> tempHeroSlotList = new List<HeroSlot>();

    public UnityAction hidePanelAction;

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
            heroSlotList.Add(heroSlot);
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