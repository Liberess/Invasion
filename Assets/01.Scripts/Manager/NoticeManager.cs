using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NoticeType
{
    Start = 0,
    Clear,
    Sell,
    NotDia,
    NotGold,
    NotStamina,
    NotParty
}

[System.Serializable]
public class NoticeMsg
{
    public string name;
    public string text;
}

public class NoticeManager : MonoBehaviour
{
    public static NoticeManager Instance { get; private set; }

    [SerializeField] private List<NoticeMsg> noticeList = new List<NoticeMsg>();

    public GameObject noticePanel;
    public Text noticeTxt;

    public bool isNegative;

    private void Awake()
    {
        Instance = this;
    }

    public void Notice(NoticeType msg)
    {
        noticeTxt.text = noticeList[(int)msg].text;

        Negative(msg);
    }

    private void Negative(NoticeType msg)
    {
        string message = msg.ToString();

        if (message.Substring(0, 3) == "Not")
            isNegative = true;
        else
            isNegative = false;

        Change();
    }

    private void Change()
    {
        if (isNegative)
            noticePanel.GetComponent<Image>().color = new Color(1, 0.3f, 0.3f);
        else
            noticePanel.GetComponent<Image>().color = new Color(0, 0.85f, 1);

        noticePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -14);
        Invoke("NoticeOff", 5f);
    }

    private void NoticeOff()
    {
        noticePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
    }
}