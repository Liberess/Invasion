using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public GameObject[] AnotPanel;

    public GameObject MyBtn;
    public GameObject MyPanel;

    private bool isClick;

    public void PanelShow()
    {
        isClick = true;

        this.GetComponent<Animator>().SetTrigger("doShow");
    }

    public void PanelHide()
    {
        isClick = false;

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

        this.GetComponent<Animator>().SetTrigger("doHide");
    }

    public void PanelQuit()
    {
        FadePanel.Instance.Fade();
        SoundManager.Instance.PlaySFX("Button");
        MyBtn.GetComponent<BtnPanel>().isClick = false;
        MyBtn.GetComponent<BtnPanel>().SpriteChange();
        MyPanel.GetComponent<Animator>().SetTrigger("doHide");
    }

    public void BtnClick()
    {
        if (isClick)
        {
            PanelHide();
            SoundManager.Instance.PlaySFX("Button");
        }
        else
        {
            PanelShow();
            SoundManager.Instance.PlaySFX("Button");
        }
    }
}