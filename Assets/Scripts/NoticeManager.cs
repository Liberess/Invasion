using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoBehaviour
{
    public static NoticeManager instance;

    public GameObject noticePanel;
    public Text noticeTxt;

    private string txt = null;
    public bool isNegative;

    private void Awake()
    {
        instance = this;
    }

    public void Notice(string msg)
    {
        switch (msg)
        {
            case "Start":
                txt = "모든 젤리를 해금하는 것이 목표입니다.";
                break;
            case "Clear":
                txt = "모든 젤리를 해금했습니다.";
                break;
            case "Sell":
                txt = "젤리를 드래그해서 주머니에 놓아 팔 수 있습니다.";
                break;
            case "NotJelatin":
                txt = "젤라틴이 부족합니다.";
                break;
            case "NotGold":
                txt = "골드가 부족합니다.";
                break;
            case "NotNum":
                txt = "젤리 수용량이 부족합니다.";
                break;
            case "NotJelly":
                txt = "젤리가 두 마리 이상 있어야 판매 가능합니다.";
                break;
        }

        noticeTxt.text = txt;

        Negative(msg);
    }

    private void Negative(string _msg)
    {
        if (_msg.Substring(0, 3) == "Not")
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }

        Change();
    }

    private void Change()
    {
        if (isNegative)
        {
            noticePanel.GetComponent<Image>().color = new Color(1, 0.3f, 0.3f);
        }
        else
        {
            noticePanel.GetComponent<Image>().color = new Color(0, 0.85f, 1);
        }

        noticePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -14);
        Invoke("NoticeOff", 5f);
    }

    private void NoticeOff()
    {
        noticePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
    }
}