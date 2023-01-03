using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[System.Serializable]
public class BuyingButton : MonoBehaviour
{
    private DataManager dataMgr;

    [SerializeField] private Text infoTxt;

    [SerializeField] private EBuyingType buyingType;
    public EBuyingType BuyingType { get => buyingType; }

    [SerializeField] private EGoodsType payGoodsType;
    public EGoodsType PayGoodsType { get => payGoodsType; }

    [SerializeField] private int price;
    public int Price { get => price; }

    [SerializeField] private int num;
    public int Num { get => num; }

    private Item item;

    private void Start()
    {
        dataMgr = DataManager.Instance;

        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += SetupItem;
        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += UpdateInfoUI;

        GetComponent<Button>().onClick.AddListener(UpdateInfoUI);
    }

    public void SetupItem()
    {
        if (dataMgr.GetItemByKey(buyingType.ToString(), out Item item))
            this.item = item;

        UpdateInfoUI();
    }

    public void UpdateInfoUI()
    {
        try
        {
            string str = "";

            if (buyingType == EBuyingType.Humal)
            {
                Debug.Log("1");
                str = string.Concat(num, "회 뽑기\n", price, payGoodsType);
            }
            else
            {
                Debug.Log("2");
                CountableItem cItem = (CountableItem)item;
                if (cItem != null)
                {
                    Debug.Log("3");
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