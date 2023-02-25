using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    public delegate void Callback();
    private Callback callbackOk = null;
    private Callback callbackYes = null;
    private Callback callbackNo = null;

    [SerializeField] private GameObject PopUpPanel;
    [SerializeField] private RectTransform ContentsPanel;
    [SerializeField] private GameObject[] PopUpTitles;
    [SerializeField] private GameObject MessageBox;
    [SerializeField] private GameObject OneBtn;
    [SerializeField] private GameObject YesOrNoBtn;

    private Button OkBtn;
    private Button YesBtn;
    private Button NoBtn;

    private Animator PopUpAnim;

    private EPopUpResponse thisResult;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        PopUpAnim = PopUpPanel.GetComponentInChildren<Animator>();

        // 버튼 배정
        OkBtn = OneBtn.GetComponent<Button>();
        YesBtn = YesOrNoBtn.transform.Find("YesBtn").gameObject.GetComponent<Button>();
        NoBtn = YesOrNoBtn.transform.Find("NoBtn").gameObject.GetComponent<Button>();

        // 기본 리스너 추가
        OkBtn.onClick.AddListener(() => PopUpClose(OkBtn.gameObject.name));
        YesBtn.onClick.AddListener(() => PopUpClose(YesBtn.gameObject.name));
        NoBtn.onClick.AddListener(() => PopUpClose(NoBtn.gameObject.name));
    }

    private void DialogInit() // 초기화
    {
        PopUpPanel.SetActive(false);

        for (int i = 0; i < PopUpTitles.Length; i++)
            PopUpTitles[i].SetActive(false);
    }

    public void PopUp(string message, EPopUpType type)
    {
        SetCallback(null, EPopUpResponse.Ok);

        DialogInit();
        PopUpPanel.SetActive(true);
        PopUpTitles[(int)type].SetActive(true);
        //MessageBox.GetComponent<TMP_Text>().text = message;
        MessageBox.GetComponent<Text>().text = message;
        StartCoroutine(ResizePopUpPanelCo());

        YesOrNoBtn.SetActive(false);
        OneBtn.SetActive(true);
        OkBtn.interactable = true;

        PopUpAnim.SetTrigger("doShow");
    }

    public void PopUp(string message, EPopUpType type, Callback Function)
    {
        SetCallback(Function, EPopUpResponse.Ok);

        DialogInit();
        PopUpPanel.SetActive(true);
        PopUpTitles[(int)type].SetActive(true);
        //MessageBox.GetComponent<TMP_Text>().text = message;
        MessageBox.GetComponent<Text>().text = message;
        StartCoroutine(ResizePopUpPanelCo());

        YesOrNoBtn.SetActive(false);
        OneBtn.SetActive(true);
        OkBtn.interactable = true;

        PopUpAnim.SetTrigger("doShow");
    }

    public void YesOrNoPopUp(string message, EPopUpType type, Callback YesBtnFuncion, Callback NoBtnFuntion)
    {
        SetCallback(YesBtnFuncion, EPopUpResponse.Yes);
        SetCallback(NoBtnFuntion, EPopUpResponse.No);

        DialogInit();
        PopUpPanel.SetActive(true);
        PopUpTitles[(int)type].SetActive(true);
        //MessageBox.GetComponent<TMP_Text>().text = message;
        MessageBox.GetComponent<Text>().text = message;
        StartCoroutine(ResizePopUpPanelCo());

        OneBtn.SetActive(false);
        YesOrNoBtn.SetActive(true);
        YesBtn.interactable = true;
        NoBtn.interactable = true;

        PopUpAnim.SetTrigger("doShow");
    }

    private IEnumerator ResizePopUpPanelCo()
    {
        yield return new WaitForEndOfFrame();
        float y = 45.0f + MessageBox.GetComponent<RectTransform>().sizeDelta.y;
        ContentsPanel.sizeDelta = new Vector2(ContentsPanel.sizeDelta.x, y);
    }

    // 팝업이 닫힐 때, 지정해 둔 콜백 실행
    public void PopUpClose(string btnName)
    {
        //switch (EventSystem.current.currentSelectedGameObject.name)
        switch (btnName)
        {
            case "OkBtn":
                thisResult = EPopUpResponse.Ok;
                callbackOk?.Invoke();
                callbackOk = null;
                break;

            case "YesBtn":
                thisResult = EPopUpResponse.Yes;
                callbackYes?.Invoke();
                callbackYes = null;
                break;

            case "NoBtn":
                thisResult = EPopUpResponse.No;
                callbackNo?.Invoke();
                callbackNo = null;
                break;

            default:
                thisResult = EPopUpResponse.Error;
                break;
        }

        OkBtn.interactable = false;
        YesBtn.interactable = false;
        NoBtn.interactable = false;

        PopUpAnim.SetTrigger("doHide");
        StartCoroutine(ClosePanelCo());
    }

    // Callback 관련
    public void SetCallback(Callback call, EPopUpResponse buttontype)
    {
        switch (buttontype)
        {
            case EPopUpResponse.Ok:
                callbackOk = call;
                break;

            case EPopUpResponse.Yes:
                callbackYes = call;
                break;

            case EPopUpResponse.No:
                callbackNo = call;
                break;

            default:
                callbackOk = call;
                callbackYes = call;
                callbackNo = call;
                break;
        }
    }

    internal void YesOrNoPopUp(string v1, EPopUpType notice, object v2, object p)
    {
        throw new NotImplementedException();
    }

    private IEnumerator ClosePanelCo()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            if (PopUpAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
                && PopUpAnim.GetCurrentAnimatorStateInfo(0).IsName("Hide"))
            {
                yield return waitForEndOfFrame;
                PopUpAnim.SetTrigger("doClear");
                break;
            }

            yield return waitForEndOfFrame;
        }

        PopUpPanel.SetActive(false);
    }
}