using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantPanel : MonoBehaviour
{
    public static PlantPanel _instance;

    public Text numTxt;
    public Text clickTxt;

    public Text numGoldTxt;
    public Text clickGoldTxt;

    public GameObject numBtn;
    public GameObject clickBtn;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        NumChange();
        ClickChange();
    }

    #region Change Events
    public void NumChange()
    {
        if (DataManager.Instance.gameData.numLevel < 5)
        {
            string str = (DataManager.Instance.gameData.numLevel * 2).ToString();
            numTxt.text = string.Concat("젤리 수용량 ", str);
            numGoldTxt.text = string.Format("{0:n0}", GameManager._instance.numGoldList[DataManager.Instance.gameData.numLevel]);
        }
       else
        {
            string str = (DataManager.Instance.gameData.numLevel * 2).ToString();
            numTxt.text = string.Concat("젤리 수용량 ", str);
            numBtn.SetActive(false);
        }
    }

    public void ClickChange()
    {
        if (DataManager.Instance.gameData.clickLevel < 5)
        {
            string str = (DataManager.Instance.gameData.clickLevel * 2).ToString();
            clickTxt.text = string.Concat("클릭 생산량 x ", str);
            clickGoldTxt.text = string.Format("{0:n0}", GameManager._instance.clickGoldList[DataManager.Instance.gameData.clickLevel]);
        }
        else
        {
            string str = (DataManager.Instance.gameData.clickLevel * 2).ToString();
            clickTxt.text = string.Concat("클릭 생산량 x ", str);
            clickBtn.SetActive(false);
        }
    }
    #endregion

    #region Button Events
    public void Num()
    {
        SoundManager.Instance.PlaySFX("Button");

        if (DataManager.Instance.gameData.gold >= GameManager.Instance.numGoldList[DataManager.Instance.gameData.numLevel])
        {
            DataManager.Instance.gameData.gold -= GameManager.Instance.numGoldList[DataManager.Instance.gameData.numLevel];
            
            if (DataManager.Instance.gameData.numLevel < 5)
            {
                DataManager.Instance.gameData.numLevel++;
            }
            
            NumChange();
        }
        else
        {
            NoticeManager.instance.Notice("NotGold");
            SoundManager.Instance.PlaySFX("Fail");
        }
    }

    public void Click()
    {
        SoundManager.Instance.PlaySFX("Button");

        if (DataManager.Instance.gameData.gold >= GameManager.Instance.clickGoldList[DataManager.Instance.gameData.clickLevel])
        {
            DataManager.Instance.gameData.gold -= GameManager.Instance.clickGoldList[DataManager.Instance.gameData.clickLevel];
            
            if(DataManager.Instance.gameData.clickLevel < 5)
            {
                DataManager.Instance.gameData.clickLevel++;
            }
            
            ClickChange();
        }
        else
        {
            NoticeManager.instance.Notice("NotGold");
            SoundManager.Instance.PlaySFX("Fail");
        }
    }
    #endregion
}