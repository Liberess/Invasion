using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnPanel : MonoBehaviour
{
    public static BtnPanel Instance;

    public GameObject MyPanel;
    public GameObject OptionPanel;

    public GameObject[] AnotPanel;

    public Sprite ShowSprite;
    public Sprite HideSprite;

    public bool isShow;
    public bool isClick;

    private void Awake()
    {
        isClick = false;
        isShow = false;
        Instance = this;
    }

    private void Update()
    {
        OptionFunc();
    }

    public void OptionFunc()
    {
        if (OptionPanel.activeSelf)
        {
            Invoke("TimeOff", 0.3f);

            GameManager.Instance.isPlay = false;

            if (isClick)
            {
                PanelHide();
            }
        }
        else
        {
            GameManager.Instance.isPlay = true;

            Time.timeScale = 1;
        }
    }

    private void TimeOff()
    {
        Time.timeScale = 0;
    }
    
    public void PanelShow()
    {
        isShow = true;
        isClick = true;

        GameManager.Instance.isPlay = false;
        GameManager.Instance.isPanel = true;

        if (GameManager.Instance.isPanel)
        {
            for (int i = 0; i < AnotPanel.Length; i++)
            {
                if (AnotPanel[i].GetComponent<BtnPanel>().isShow)
                {
                    AnotPanel[i].SendMessage("PanelHide");
                }
            }
        }

        GetComponent<Image>().sprite = ShowSprite;
        MyPanel.GetComponent<Animator>().SetTrigger("doShow");
    }

    public void PanelHide()
    {
        isShow = false;
        isClick = false;

        GameManager.Instance.isPlay = true;
        GameManager.Instance.isPanel = false;

        GetComponent<Image>().sprite = HideSprite;
        MyPanel.GetComponent<Animator>().SetTrigger("doHide");
    }

    public void SpriteChange()
    {
        GetComponent<Image>().sprite = HideSprite;
    }

    public void BtnClick()
    {
        if(isClick)
        {
            PanelHide();
            SoundManager.Instance.PlaySFX("Button");
        }
        else
        {
            if(gameObject.name == "HeroBtn")
            {
                FadePanel.Instance.Fade();
            }
            PanelShow();
            SoundManager.Instance.PlaySFX("Button");
        }
    }
}