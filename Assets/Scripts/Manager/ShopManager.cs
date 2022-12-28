using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [HideInInspector] public List<Button> shopBtnList = new List<Button>();

    public GameObject shopPanel;
    private Transform shopPanelParent;

    [HideInInspector] public List<GameObject> shopPanelList = new List<GameObject>();

    private DataManager dataMgr;
    private PopUpManager popUpMgr;

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

        shopPanelList[0].gameObject.SetActive(true);
        shopBtnList[0].GetComponent<Animator>().SetBool("isHighlight", true);
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

    public void OnClickBuy(PriceComponent priceComponent)
    {
        try
        {
            DataManager.Instance.SetGoods(
                priceComponent.PayGoodsType, -priceComponent.Price);

            for (int i = 0; i < priceComponent.Num; i++)
            {

            }
        }
        catch (Exception exp)
        {
            if (exp.Message.Contains("Insufficient"))
            {
                string msg = string.Concat(priceComponent.PayGoodsType.ToString(), " 재화가 부족합니다.");
                popUpMgr.PopUp(msg, EPopUpType.Caution);
            }

            return;
        }
    }
}