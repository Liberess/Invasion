using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    [SerializeField] private Text lvTxt;
    [SerializeField] private Image heroImg;

    private Button btn;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text partyMenuTxt;

    [SerializeField] private HumalData heroStatus;
    public HumalData HeroStatus { get => heroStatus; }

    [SerializeField] private bool isParty = false;

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

    public void HeroStatusSetup(HumalData humalData)
    {
        heroStatus = humalData;
        heroImg.sprite = humalData.sprite;
    }

    public void SlotDragEvent()
    {
        UIManager.Instance.HideHeroInfoPanelAction();
    }

    public void UpdateSlotImage()
    {
        if (heroStatus.IsLeader)
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

        //UIManager.Instance.HideHeroInfoPanelAction();

        //Vector3 newPos = transform.position + new Vector3(rt.rect.x, rt.rect.y, 0f);
        Vector3 newPos = transform.position + menuPos;
        //menuPanel.GetComponent<RectTransform>().transform.position = newPos;
        menuPanel.transform.Find("MenuPanel").position = newPos;
        menuPanel.SetActive(true);

        UIManager.Instance.UpdateHeroDetailInfo(heroStatus.Data.ID);

        if (isParty)
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