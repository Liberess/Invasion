using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private DataManager dataMgr;
    private PopUpManager popUpMgr;

    private List<Button> shopBtnList = new List<Button>();

    public GameObject shopPanel;
    private Transform shopPanelParent;

    public List<GameObject> shopPanelList = new List<GameObject>();

    private List<BuyingButton> buyingBtnList = new List<BuyingButton>();
    private BuyingButton currentBuyingBtn;

    public UnityAction OnShopInitAction;

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
        buyingBtnList.Clear();
        for (int i = 0; i < shopPanelParent.transform.childCount; i++)
        {
            shopPanelList.Add(shopPanelParent.GetChild(i).gameObject);

            var buyingBtns = shopPanelParent.GetChild(i).GetComponentsInChildren<BuyingButton>();
            foreach(var btn in buyingBtns)
            {
                buyingBtnList.Add(btn);
                OnShopInitAction += btn.SetupItem;
                OnShopInitAction += btn.UpdateInfoUI;
            }
        }

        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += OnShopInitAction;

        OnClickShopMenu(EShopMenu.Humal);
    }

    public void OnClickShopMenu(EShopMenu shopMenu)
    {
        for (int i = 0; i < shopBtnList.Count; i++)
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

    private void SetupBuyingButton()
    {

    }

    public void OnClickBuy(BuyingButton buyingBtn)
    {
        currentBuyingBtn = buyingBtn;

        try
        {
            if(dataMgr.GetItemByKey(buyingBtn.BuyingType.ToString(), out Item item))
            {
                if(Utility.DownCastingItem(item, out CountableItem cItem))
                {
                    if (cItem.TodayBuyingAmount < cItem.MaxBuyingAmount)
                        StartCoroutine(BuyCo());
                    else
                        popUpMgr.PopUp("하루 구매 횟수를 모두 소모했습니다!", EPopUpType.Caution);
                }
                else
                {
                    StartCoroutine(BuyCo());
                }
            }
            else
            {
                StartCoroutine(BuyCo());
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

    private IEnumerator BuyCo()
    {
        if (currentBuyingBtn != null)
        {
            dataMgr.SetGoodsAmount(currentBuyingBtn.PayGoodsType, -currentBuyingBtn.Price);

            for (int i = 0; i < currentBuyingBtn.Num; i++)
                yield return StartCoroutine(BuyItemCo(currentBuyingBtn.BuyingType));

            currentBuyingBtn.UpdateInfoUI();
            currentBuyingBtn = null;
        }

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