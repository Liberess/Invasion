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

    [SerializeField] private UnitStatus myStatus;
    public UnitStatus MyStatus { get => myStatus; }

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
            partyMenuTxt = menuPanel.transform.Find("PartyMenuBtn").Find("Text").GetComponent<Text>();

        menuPos = transform.Find("MenuPos").transform.localPosition;

        UIManager.Instance.HideHeroInfoPanelAction += () => menuPanel.SetActive(false);

        btn.onClick.AddListener(OnClickEvent);
    }

    public void SlotDragEvent()
    {
        UIManager.Instance.HideHeroInfoPanelAction();
    }    

    private void OnClickEvent()
    {
        var drag = GetComponent<Draggable>();

        if (drag != null && drag.CanDrag)
            return;

        UIManager.Instance.HideHeroInfoPanelAction();

        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            //Vector3 newPos = transform.position + new Vector3(rt.rect.x, rt.rect.y, 0f);
            Vector3 newPos = transform.position + menuPos;
            //menuPanel.GetComponent<RectTransform>().transform.position = newPos;
            menuPanel.transform.position = newPos;
            menuPanel.SetActive(true);

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

    public void UnitSetup(UnitStatus status)
    {
        myStatus = status;
        heroImg.sprite = myStatus.mySprite;
    }
}