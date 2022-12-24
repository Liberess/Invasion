using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum EBtnType
{
    New,
    Load,
    Save,
    Option,
    Back,
    Main,
    Exit,
    Restart,
    BackDungeon,
    KarmaDungeon
}

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
            case EBtnType.New:
                //SceneLoad.LoadSceneHandle(3, 1);
                break;
            case EBtnType.Load:
                //gameManager.GameLoad();
                break;
            case EBtnType.Save:
                //gameManager.GameSave();
                break;
            case EBtnType.Main:
                //SceneLoad.LoadSceneHandle(1, 0);
                break;
            case EBtnType.Exit:
                Application.Quit();
                break;
            case EBtnType.Restart:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case EBtnType.BackDungeon:
                //SceneManager.LoadScene(0);
                break;
            case EBtnType.KarmaDungeon:
                //SceneManager.LoadScene("Karma");
                break;
        }

        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData) => btnScale.localScale = defaultScale * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => btnScale.localScale = defaultScale;
}