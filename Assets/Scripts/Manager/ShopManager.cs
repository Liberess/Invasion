using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private DataManager dataMgr;
    private PopUpManager popUpMgr;

    [HideInInspector] public List<Button> shopBtnList = new List<Button>();

    public GameObject shopPanel;
    private Transform shopPanelParent;

    [HideInInspector] public List<GameObject> shopPanelList = new List<GameObject>();
    //private buyingBtns = 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;
        popUpMgr = PopUpManager.Instance;

        if (!shopPanel)
            shopPanel = GameObject.Find("UICanvas").transform.Find("ShopPanel").gameObject;

        shopBtnList.Clear();
        var btnParent = shopPanel.transform.Find("ShopBtnGroup");
        for (int i = 0; i < btnParent.childCount; i++)
        {
            int index = i;
            shopBtnList.Add(btnParent.GetChild(i).GetComponent<Button>());
            shopBtnList[i].onClick.AddListener(() => OnClickShopMenu((EShopMenu)index));
        }

        shopPanelParent = shopPanel.transform.Find("PickPanelGroup");

        shopPanelList.Clear();
        for (int i = 0; i < shopPanelParent.transform.childCount; i++)
            shopPanelList.Add(shopPanelParent.GetChild(i).gameObject);

        OnClickShopMenu(EShopMenu.Humal);
    }

    public void OnClickShopMenu(EShopMenu shopMenu)
    {
        for(int i = 0; i < shopBtnList.Count; i++)
        {
            if(i == (int)shopMenu)
            {
                shopPanelList[i].gameObject.SetActive(true);
                shopBtnList[i].GetComponent<Animator>().SetBool("isHighlight", true);
            }
            else
            {
                shopPanelList[i].gameObject.SetActive(false);
                shopBtnList[i].GetComponent<Animator>().SetBool("isHighlight", false);
            }
        }
    }

    public void OnClickBuy(BuyingButton buyingBtn)
    {
        try
        {
            if(dataMgr.GetItemByKey(buyingBtn.BuyingType.ToString(), out Item item))
            {
                if(Utility.DownCastingItem(item, out CountableItem cItem))
                {
                    if (cItem.TodayBuyingAmount < cItem.MaxBuyingAmount)
                        StartCoroutine(BuyCo(buyingBtn));
                    else
                        popUpMgr.PopUp("하루 구매 횟수를 모두 소모했습니다!", EPopUpType.Caution);
                }
                else
                {
                    StartCoroutine(BuyCo(buyingBtn));
                }
            }
            else
            {
                StartCoroutine(BuyCo(buyingBtn));
            }
        }
        catch (Exception exp)
        {
            if (exp.Message.Contains("Insufficient"))
            {
                string msg = string.Concat(buyingBtn.PayGoodsType.ToString(), " 재화가 부족합니다.");
                popUpMgr.PopUp(msg, EPopUpType.Caution);
            }

            return;
        }
    }

    private IEnumerator BuyCo(BuyingButton buyingBtn)
    {
        dataMgr.SetGoodsAmount(buyingBtn.PayGoodsType, -buyingBtn.Price);

        for (int i = 0; i < buyingBtn.Num; i++)
            yield return StartCoroutine(BuyItemCo(buyingBtn.BuyingType));

        buyingBtn.UpdateInfoUI();

        yield return null;

    }

    private IEnumerator BuyItemCo(EBuyingType buyingType)
    {
        if(buyingType == EBuyingType.Humal)
        {

        }
        else
        {
            var itemData = dataMgr.GetItemDataByKey(buyingType.ToString());
            ConsumeItem item = new ConsumeItem((ConsumeItemData)itemData, 1);
            dataMgr.AddInventoryItem(item, 1);
        }

        yield return null;
    }
}