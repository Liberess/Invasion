using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyingButton : MonoBehaviour
{
    private DataManager dataMgr;

    [SerializeField] private EBuyingType buyingType;
    public EBuyingType BuyingType { get => buyingType; }

    [SerializeField] private EGoodsType payGoodsType;
    public EGoodsType PayGoodsType { get => payGoodsType; }

    [SerializeField] private int price;
    public int Price { get => price; }

    [SerializeField] private int num;
    public int Num { get => num; }

    private Item item;
    private Text infoTxt;

    private void Awake()
    {
        infoTxt = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;
    }

    public void SetupItem()
    {
        if (DataManager.Instance.GetItemByKey(buyingType.ToString(), out Item item))
            this.item = item;
    }

    public void UpdateInfoUI()
    {
        try
        {
            string str = "";

            if (buyingType == EBuyingType.Humal)
            {
                str = string.Concat(num, "회 뽑기\n", price, " ", payGoodsType);
            }
            else
            {
                CountableItem cItem = item as CountableItem;
                if (cItem != null)
                {
                    str = string.Concat(
                        price,
                        " ",
                        payGoodsType.ToString(),
                        "\n",
                        cItem.TodayBuyingAmount,
                        "/",
                        cItem.MaxBuyingAmount
                    );
                }
            }

            infoTxt.text = str;
        }
        catch(Exception ex)
        {
            PopUpManager.Instance.PopUp(ex.Message, EPopUpType.Caution);
        }
    }
}