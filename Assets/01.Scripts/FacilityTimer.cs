using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityTimer : MonoBehaviour
{
    private DataManager dataMgr;

    public Button upBtn;
    public Button rewardBtn;
    public Text lvTxt;

    public int ID;

    public float myTime;
    private float limitTime;
    public float sliderTime;

    private void Awake()
    {
        dataMgr = DataManager.Instance;

        if (dataMgr.gameData.facilLimitTime == null)
        {
            switch (ID)
            {
                case 0: limitTime = 10f; break;        //10초
                case 1: limitTime = 60f; break;        //1분
                case 2: limitTime = 600f; break;      //10분
                case 3: limitTime = 1800f; break;    //30분
                case 4: limitTime = 3600f; break;    //1시간
                case 5: limitTime = 14400f; break;  //4시간
                case 6: limitTime = 43200f; break;  //12시간
            }

            myTime = limitTime;
        }
        else
        {
            switch (ID)
            {
                case 0: myTime = 10f; break;        //10초
                case 1: myTime = 60f; break;        //1분
                case 2: myTime = 600f; break;      //10분
                case 3: myTime = 1800f; break;    //30분
                case 4: myTime = 3600f; break;    //1시간
                case 5: myTime = 14400f; break;  //4시간
                case 6: myTime = 43200f; break;  //12시간
            }

            limitTime = dataMgr.gameData.facilLimitTime[ID];
            sliderTime = dataMgr.gameData.facilSliderTime[ID];
        }

        ChangeTimerTxt();
    }

    private void Start()
    {
        FacilityManager.Instance.facilSliders[ID].maxValue = myTime;
        
        SetLvTxt();
        SetGoldTxt();
    }

    private void Update()
    {
        SaveLimitTime();
        ChangeTimerTxt();
    }

    private void FixedUpdate()
    {
        Timer();
        SetSliderValue();
    }

    public void SetSliderValue()
    {
        FacilityManager.Instance.facilSliders[ID].value = sliderTime;
    }

    public void SaveLimitTime()
    {
        dataMgr.gameData.facilLimitTime[ID] = limitTime;
        dataMgr.gameData.facilSliderTime[ID] = sliderTime;
    }

    #region 타이머
    private void Timer()
    {
        if(dataMgr.gameData.facilUnlockList[ID])
        {
            if (limitTime >= 0)
            {
                limitTime -= Time.deltaTime;
            }
            else
            {
                rewardBtn.interactable = true;
                rewardBtn.image.enabled = true;
                rewardBtn.transform.Find("Text").gameObject.SetActive(true);
            }

            if(sliderTime <= myTime)
            {
                sliderTime += Time.deltaTime;
            }
        }
    }

    public void ChangeTimerTxt()
    {
        int minute = (int)limitTime / 60;
        int hour = minute / 60;
        int second = (int)limitTime % 60;
        minute = minute % 60;
        
        GetComponent<Text>().text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
    }
    #endregion

    public void FacilLvUp()
    {
        dataMgr.gameData.facilLevelList[ID]++;

        SetLvTxt();
        SetGoldTxt();

        dataMgr.gameData.goods[(int)Goods.Gold].count -=
            (FacilityManager.Instance.facilGoldList[ID] * (dataMgr.gameData.facilLevelList[ID] + 1));
    }

    public void SetLvTxt()
    {
        if (ID != 2)
        {
            lvTxt.text = FacilityManager.Instance.facilNameList[ID] + " Lv." + dataMgr.gameData.facilLevelList[ID].ToString();
        }
        else
        {
            lvTxt.text = "Lv." + dataMgr.gameData.facilLevelList[ID].ToString();
        }
    }

    public void SetGoldTxt()
    {
        /* FacilityManager.Instance.facilSliders[ID].transform.Find("MaxTxt").GetComponent<Text>().text
            = (FacilityManager.Instance.facilGoldList[ID] * (dataMgr.gameData.facilLevelList[ID] + 1)).ToString(); */

        string maxGold = (FacilityManager.Instance.facilGoldList[ID] *
            (dataMgr.gameData.facilLevelList[ID] + 1)).ToString();
        dataMgr.gameData.facilGold[ID] = int.Parse(maxGold);
        FacilityManager.Instance.facilSliders[ID].transform.Find("MaxTxt").GetComponent<Text>().text
            = maxGold;

        string upGold = (FacilityManager.Instance.facilGoldList[ID] * (dataMgr.gameData.facilLevelList[ID] + 1)).ToString();
        upBtn.transform.Find("PlusGoldTxt").GetComponent<Text>().text
            = "+" + upGold;

        string needGold = (FacilityManager.Instance.facilGoldList[ID] * (dataMgr.gameData.facilLevelList[ID] + 1)).ToString();
        upBtn.transform.Find("NeedGoldTxt").GetComponent<Text>().text
            = needGold;
    }

    public void GetReward()
    {
        sliderTime = 0;
        limitTime = myTime;

        rewardBtn.interactable = false;
        rewardBtn.image.enabled = false;
        rewardBtn.transform.Find("Text").gameObject.SetActive(false);

        dataMgr.gameData.goods[(int)Goods.Gold].count +=
            FacilityManager.Instance.facilGoldList[ID] * (dataMgr.gameData.facilLevelList[ID] + 1);
    }

    private void OnApplicationPause(bool pause)
    {
        SetSliderValue();
        SaveLimitTime();
    }

    private void OnApplicationQuit()
    {
        SetSliderValue();
        SaveLimitTime();
    }
}