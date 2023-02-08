using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EBtnType crtType;
    public Transform btnScale;

    private Button btn;
    private Vector3 defaultScale;

    private void OnEnable()
    {
        if (btnScale)
            btnScale.localScale = defaultScale != Vector3.zero ? defaultScale : Vector3.one;
    }

    private void OnDisable()
    {
        if (btnScale)
            btnScale.localScale = defaultScale != Vector3.zero ? defaultScale : Vector3.one;
    }

    private void Awake()
    {
        if (!btnScale)
            btnScale = GetComponent<Transform>();
    }

    private void Start()
    {
        defaultScale = btnScale.localScale;
        if (defaultScale == Vector3.zero)
            defaultScale = Vector3.one;

        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => StartCoroutine(ButtonClickCo()));
    }

    private IEnumerator ButtonClickCo()
    {
        SoundManager.Instance.PlaySFX("Button");

        switch (crtType)
        {
            case EBtnType.GPGSLogin:
                PlayFabManager.Instance.StartCoroutine(PlayFabManager.Instance.GoogleLogInCo());
                break;

            case EBtnType.GPGSLogout:
                PlayFabManager.Instance.StartCoroutine(PlayFabManager.Instance.GoogleLogOutCo());
                break;

            case EBtnType.QuitGame:
                Application.Quit();
                break;

            case EBtnType.InitializedData:
                DataManager.Instance.InitializedData();
                //SceneLoad.LoadSceneHandle(3, 1);
                break;

            case EBtnType.Option:
                break;

            case EBtnType.CloseUI:
                UIManager.Instance.HidePanelAction();
                transform.GetComponentInParent<Animator>().SetTrigger("doHide");
                yield return new WaitForSeconds(1f);
                //transform.parent.gameObject.SetActive(false);
                break;

            case EBtnType.GoToMain:
                //SceneLoad.LoadSceneHandle(1, 0);
                break;

            case EBtnType.RestartBattle:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;

            case EBtnType.PlayFabLogin:
                PlayFabManager.Instance.TestLogin();
                //playFabMgr.StartCoroutine(playFabMgr.PlayFabLogInCo());
                break;

            case EBtnType.Buy:
                if (TryGetComponent(out BuyingButton price))
                    ShopManager.Instance.OnClickBuy(price);
                break;
        }

        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData) => btnScale.localScale = defaultScale * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => btnScale.localScale = defaultScale;
}