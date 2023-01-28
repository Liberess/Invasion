using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    [Header("==== Unlock Group ====")]
    [SerializeField] private Text lvTxt;
    [SerializeField] private Image heroImg;

    private Button btn;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text partyMenuTxt;

    [SerializeField] private UnitData humalData;
    public UnitData HumalData { get => humalData; }

    [Space(5), Header("==== Lock Group ====")]
    [SerializeField] private GameObject lockGroup;
    [SerializeField] private Image pieceImg;
    [SerializeField] private Image piecefillImg;
    [SerializeField] private Text pieceAmountTxt;

    private bool isParty = false;

    private Vector3 menuPos;
    private RectTransform rt;

    private void Awake()
    {
        btn = GetComponent<Button>();
        rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (lvTxt == null)
            lvTxt = transform.Find("LvTxt").GetComponent<Text>();

        if (heroImg == null)
            heroImg = transform.Find("HeroImg").GetComponent<Image>();

        if (menuPanel == null)
            menuPanel = GameObject.Find("UICanvas").transform.Find("HeroPanel").Find("MenuPanel").gameObject;

        if (partyMenuTxt == null)
            partyMenuTxt = menuPanel.transform.Find("MenuPanel").Find("PartyMenuBtn").Find("Text").GetComponent<Text>();

        menuPos = transform.Find("MenuPos").transform.localPosition;

        btn.onClick.AddListener(OnClickEvent);
        //btn.onClick.AddListener(OnClickEvent);
        //menuPanel.GetComponent<Button>().onClick.AddListener(OnClickOther);
    }

    public void HumalDataSetup(UnitData unitData)
    {
        humalData = unitData;

        if (DataManager.Instance.HumalData.humalSpriteDic.ContainsKey(unitData.ID))
            heroImg.sprite = DataManager.Instance.HumalData.humalSpriteDic[unitData.ID];

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if(lockGroup.activeSelf)
        {
            if (DataManager.Instance.TryGetHumalPieceAmount(humalData.ID, out int amount))
            {
                pieceAmountTxt.text = string.Concat(amount, "/", 100);
                piecefillImg.fillAmount = amount / 100.0f;
            }
        }
        else
        {
            lvTxt.text = humalData.Level.ToString();
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
            UpdateSlot();
        }
        else
        {
            btn.interactable = false;
            heroImg.color = Color.black;
            lvTxt.gameObject.SetActive(false);
            lockGroup.SetActive(true);
            UpdateSlot();
        }
    }

    public void SlotDragEvent()
    {
        UIManager.Instance.HideHeroInfoPanelAction();
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
            menuPanel.SetActive(false);
    }

    private void OnClickEvent()
    {
        var drag = GetComponent<Draggable>();

        if (drag != null && drag.CanDrag)
            return;

        Vector3 newPos = transform.position + menuPos;
        menuPanel.transform.Find("MenuPanel").position = newPos;
        menuPanel.SetActive(true);

        UIManager.Instance.UpdateHeroDetailInfo(humalData.ID, humalData.IsParty);

        if (humalData.IsParty)
        {
            partyMenuTxt.text = "파티 해제";
            menuPanel.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else
        {
            partyMenuTxt.text = "파티 추가";
            menuPanel.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}