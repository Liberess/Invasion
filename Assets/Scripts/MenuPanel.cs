using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public GameObject[] AnotPanel;

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