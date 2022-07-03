using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BTNType
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
    public BTNType currentType;

    public Transform buttonScale;

    public GameManager gameManager;

    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        switch (currentType)
        {
            case BTNType.New:
               //SceneLoad.LoadSceneHandle(3, 1);
                break;
            case BTNType.Load:
                //gameManager.GameLoad();
                break;
            case BTNType.Save:
                //gameManager.GameSave();
                break;
            case BTNType.Option:
                CanvasGroupOn(optionGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(optionGroup);
                break;
            case BTNType.Main:
                //SceneLoad.LoadSceneHandle(1, 0);
                break;
            case BTNType.Exit:
                Application.Quit();
                break;
            case BTNType.Restart:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case BTNType.BackDungeon:
                //SceneManager.LoadScene(0);
                break;
            case BTNType.KarmaDungeon:
                //SceneManager.LoadScene("Karma");
                break;
        }
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}