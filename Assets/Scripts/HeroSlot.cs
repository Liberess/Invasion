using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    private UIManager uiMgr;

    [SerializeField] private AssetReference starYellowImgRef;
    [SerializeField] private AssetReference starPurpleImgRef;
    
    [Header("==== Unlock Group ====")]
    [SerializeField] private Text lvTxt;
    [SerializeField] private Image heroImg;
    [SerializeField] private GameObject gradeGrid;
    [SerializeField] private Image[] gradeImgs = new Image[5];

    private Button btn;

    [SerializeField] private UnitData humalData;
    public UnitData HumalData { get => humalData; }

    private int weight = 0;
    public int Weight
    {
        get
        {
            if(humalData.IsUnlock) return weight + 20;
            if (unlockBtn.gameObject.activeSelf) return weight + 10;
            return weight;
        }
        
        set => weight = value;
    }

    [Space(5), Header("==== Lock Group ====")]
    [SerializeField] private GameObject lockGroup;
    [SerializeField] private Image pieceImg;
    [SerializeField] private Image piecefillImg;
    [SerializeField] private Text pieceAmountTxt;
    [SerializeField] private Button unlockBtn;

    private Draggable drag;

    private Vector3 menuPos;
    private RectTransform rt;

    private void Awake()
    {
        drag = GetComponent<Draggable>();
        btn = GetComponent<Button>();
        rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        uiMgr = UIManager.Instance;
        
        if (lvTxt == null)
            lvTxt = transform.Find("LvTxt").GetComponent<Text>();

        if (heroImg == null)
            heroImg = transform.Find("HeroImg").GetComponent<Image>();

        menuPos = transform.Find("MenuPos").transform.localPosition;

        btn.onClick.AddListener(OnClickEvent);

        unlockBtn.onClick.AddListener(OnClickUnlock);
        unlockBtn.gameObject.SetActive(false);

        foreach (var img in gradeImgs)
        {
            img.sprite = null;
            img.enabled = false;
        }
        
        //btn.onClick.AddListener(OnClickEvent);
        //menuPanel.GetComponent<Button>().onClick.AddListener(OnClickOther);
    }

    public void HumalDataSetup(UnitData unitData)
    {
        humalData = unitData;

        if (DataManager.Instance.HumalData.humalSpriteDic.ContainsKey(humalData.ID))
            heroImg.sprite = DataManager.Instance.HumalData.humalSpriteDic[humalData.ID];

        UpdateSlot();
    }

    public void UpdateHumalData(UnitData unitData) => humalData = unitData;

    public async UniTaskVoid UpdateSlot()
    {
        if(lockGroup.activeSelf)
        {
            if (DataManager.Instance.TryGetHumalPieceAmount(humalData.ID, out int amount))
            {
                if (amount >= 100)
                    unlockBtn.gameObject.SetActive(true);
                
                pieceAmountTxt.text = string.Concat(amount, "/", 100);
                piecefillImg.fillAmount = amount / 100.0f;
            }
            else
            {
                pieceAmountTxt.text = "0/0";
                piecefillImg.fillAmount = 0.0f;
            }
        }
        else
        {
            lvTxt.text = humalData.Level.ToString();

            await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset(starYellowImgRef) != null);
            await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset(starPurpleImgRef) != null);
            
            for (int i = 0; i < HumalData.Grade; i++)
            {
                if (i < 5)
                {
                    gradeImgs[i].enabled = true;
                    gradeImgs[i].sprite = ResourcesManager.Instance.LoadAsset(starYellowImgRef) as Sprite;
                }
                else
                {
                    gradeImgs[i-5].enabled = true;
                    gradeImgs[i-5].sprite = ResourcesManager.Instance.LoadAsset(starPurpleImgRef) as Sprite;
                }
            }
        }
    }

    public void SetEnabledHumalSlot(bool enabled)
    {
        if(enabled)
        {
            btn.interactable = true;
            heroImg.color = Color.white;
            lvTxt.gameObject.SetActive(true);
            lockGroup.SetActive(false);
            gradeGrid.SetActive(true);
            UpdateSlot();
        }
        else
        {
            btn.interactable = false;
            heroImg.color = Color.black;
            lvTxt.gameObject.SetActive(false);
            lockGroup.SetActive(true);
            gradeGrid.SetActive(false);
            UpdateSlot();
        }
    }

    public void SlotDragEvent()
    {
        uiMgr.HideHeroInfoPanelAction();
    }

    public void UpdateSlotImage()
    {
        if (humalData.IsLeader)
            GetComponent<Image>().color = new Color(250, 250, 130);
        else
            GetComponent<Image>().color = Color.white;
    }

    private void OnClickOther()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            uiMgr.HeroMenuPanel.SetActive(false);
    }

    public void SetEnabledDraggable(bool enabled) => drag.enabled = enabled;

    private void OnClickEvent()
    {
        try
        {
            if (drag != null && drag.CanDrag)
                return;

            Vector3 newPos = rt.position + menuPos;
            uiMgr.HeroMenuPanel.transform.GetChild(0).position = newPos;
            
            uiMgr.UpdateHeroDetailInfo(humalData);
            uiMgr.SetCurrentHumalSlot(this);
            
            if (humalData.IsParty)
            {
                uiMgr.partyMenuTxt.text = "파티 해제";
                uiMgr.HeroMenuPanel.transform.GetChild(0).localScale = new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                uiMgr.partyMenuTxt.text = "파티 추가";
                uiMgr.HeroMenuPanel.transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
            }
            
            uiMgr.HeroMenuPanel.SetActive(true);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void OnClickUnlock()
    {
        DataManager.Instance.SubtractHumalPiece(humalData.ID, 100);
        DataManager.Instance.AddNewHumal(humalData.ID);
        unlockBtn.gameObject.SetActive(false);
    }
}