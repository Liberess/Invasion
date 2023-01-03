using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkFacility : MonoBehaviour
{
    private DataManager dataMgr;

    public FacilityData Data { get; private set; }

    public Button upBtn;
    public Button rewardBtn;
    public Text lvTxt;

    [SerializeField] private int m_makingAmount;
    public int MakingAmount => m_makingAmount;

    public float myTime;
    private float limitTime;
    public float sliderTime;



    private void Awake()
    {
        dataMgr = DataManager.Instance;

        if (dataMgr.GameData.facilLimitTime == null)
        {
            switch (Data.ID)
            {
                case 0: limitTime = 10f; break;        //10��
                case 1: limitTime = 60f; break;        //1��
                case 2: limitTime = 600f; break;      //10��
                case 3: limitTime = 1800f; break;    //30��
                case 4: limitTime = 3600f; break;    //1�ð�
                case 5: limitTime = 14400f; break;  //4�ð�
                case 6: limitTime = 43200f; break;  //12�ð�
            }

            myTime = limitTime;
        }
        else
        {
            switch (Data.ID)
            {
                case 0: myTime = 10f; break;        //10��
                case 1: myTime = 60f; break;        //1��
                case 2: myTime = 600f; break;      //10��
                case 3: myTime = 1800f; break;    //30��
                case 4: myTime = 3600f; break;    //1�ð�
                case 5: myTime = 14400f; break;  //4�ð�
                case 6: myTime = 43200f; break;  //12�ð�
            }

            limitTime = dataMgr.GameData.facilLimitTime[Data.ID];
            sliderTime = dataMgr.GameData.facilSliderTime[Data.ID];
        }

        ChangeTimerTxt();
    }

    private void Start()
    {
        FacilityManager.Instance.facilSliders[Data.ID].maxValue = myTime;

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
        FacilityManager.Instance.facilSliders[Data.ID].value = sliderTime;
    }

    public void SaveLimitTime()
    {
        dataMgr.GameData.facilLimitTime[Data.ID] = limitTime;
        dataMgr.GameData.facilSliderTime[Data.ID] = sliderTime;
    }

    #region Ÿ�̸�
    private void Timer()
    {
        if (dataMgr.GameData.facilUnlockList[Data.ID])
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

            if (sliderTime <= myTime)
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
        dataMgr.GameData.facilLevelList[Data.ID]++;

        SetLvTxt();
        SetGoldTxt();

        dataMgr.GameData.GoodsList[(int)EGoodsType.Gold].count -=
            (FacilityManager.Instance.facilGoldList[Data.ID] * (dataMgr.GameData.facilLevelList[Data.ID] + 1));
    }

    public void SetLvTxt()
    {
        if (Data.ID != 2)
        {
            lvTxt.text = FacilityManager.Instance.facilNameList[Data.ID] + " Lv." + dataMgr.GameData.facilLevelList[Data.ID].ToString();
        }
        else
        {
            lvTxt.text = "Lv." + dataMgr.GameData.facilLevelList[Data.ID].ToString();
        }
    }

    public void SetGoldTxt()
    {
        /* FacilityManager.Instance.facilSliders[Data.ID].transform.Find("MaxTxt").GetComponent<Text>().text
            = (FacilityManager.Instance.facilGoldList[Data.ID] * (dataMgr.GameData.facilLevelList[Data.ID] + 1)).ToString(); */

        string maxGold = (FacilityManager.Instance.facilGoldList[Data.ID] *
            (dataMgr.GameData.facilLevelList[Data.ID] + 1)).ToString();
        dataMgr.GameData.facilGold[Data.ID] = int.Parse(maxGold);
        FacilityManager.Instance.facilSliders[Data.ID].transform.Find("MaxTxt").GetComponent<Text>().text
            = maxGold;

        string upGold = (FacilityManager.Instance.facilGoldList[Data.ID] * (dataMgr.GameData.facilLevelList[Data.ID] + 1)).ToString();
        upBtn.transform.Find("PlusGoldTxt").GetComponent<Text>().text
            = "+" + upGold;

        string needGold = (FacilityManager.Instance.facilGoldList[Data.ID] * (dataMgr.GameData.facilLevelList[Data.ID] + 1)).ToString();
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

        dataMgr.GameData.GoodsList[(int)EGoodsType.Gold].count +=
            FacilityManager.Instance.facilGoldList[Data.ID] * (dataMgr.GameData.facilLevelList[Data.ID] + 1);
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