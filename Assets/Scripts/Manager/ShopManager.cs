using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    [SerializeField] private List<HumalPickDBEntity> humalPickDBList = new List<HumalPickDBEntity>();

    public event Action OnShopInitAction;

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

#if UNITY_EDITOR
        UpdateHumalPickDB();
#endif

        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += OnShopInitAction;
        PlayFabManager.Instance.OnPlayFabLoginSuccessAction += UpdateHumalPickDB;

        OnClickShopMenu(EShopMenu.Humal);
    }

    private void UpdateHumalPickDB()
    {
        Addressables.LoadAssetAsync<HumalPickDB>(Tags.HumalPickDBLabel).Completed +=
        handle =>
        {
            foreach(var db in handle.Result.Entities)
            {
                if (!humalPickDBList.Contains(db))
                    humalPickDBList.Add(db);
            }

            humalPickDBList = humalPickDBList.OrderBy(x => x.id).ToList();
        };
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

    public void OnClickBuy(BuyingButton buyingBtn)
    {
        currentBuyingBtn = buyingBtn;

        if (dataMgr.GetItemByKey(buyingBtn.BuyingType.ToString(), out Item item))
        {
            if (Utility.DownCastingItem(item, out CountableItem cItem))
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

    private IEnumerator BuyCo()
    {
        if (currentBuyingBtn != null)
        {
            if(dataMgr.SetCurrencyAmount(currentBuyingBtn.PayGoodsType, -currentBuyingBtn.Price))
            {
                for (int i = 0; i < currentBuyingBtn.Num; i++)
                    yield return StartCoroutine(BuyItemCo(currentBuyingBtn.BuyingType));

                currentBuyingBtn.UpdateInfoUI();
                currentBuyingBtn = null;
            }
        }

        yield return null;
    }

    private IEnumerator BuyItemCo(EBuyingType buyingType)
    {
        if(buyingType == EBuyingType.Humal)
        {
            yield return StartCoroutine(PickUpHumal());
        }
        else
        {
            var itemData = dataMgr.GetItemDataByKey(buyingType.ToString());
            ConsumeItem item = new ConsumeItem((ConsumeItemData)itemData, 1);
            dataMgr.AddInventoryItem(item, 1);
        }

        yield return null;
    }

    private IEnumerator PickUpHumal()
    {
        var index = UnityEngine.Random.Range(0, humalPickDBList.Count);
        var entity = humalPickDBList[index];

        var picker = new Rito.WeightedRandomPicker<string>();
        picker.Add(
            (nameof(entity.piece_10), entity.piece_10),
            (nameof(entity.piece_20), entity.piece_20),
            (nameof(entity.piece_50), entity.piece_50),
            (nameof(entity.humal), entity.humal)
        );

        var pick = picker.GetRandomPick();
        if (pick.Contains("humal"))
        {
            if (dataMgr.IsContainsInHumalList(index))
                dataMgr.AddHumalPiece(entity.id, 50);
            else
                dataMgr.AddNewHumal(index);
        }
        else
        {
            int amount = int.Parse(pick.Substring(pick.IndexOf('_') + 1));
            dataMgr.AddHumalPiece(entity.id, amount);
        }

        yield return null;
    }
}