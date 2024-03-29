﻿using System;
using System.Text;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayFabManager playFabMgr;
    private DataManager dataMgr;

    private string url = "www.naver.com";

    private int stopWatch;

    private int _dia, _gold, _drink;

    private int reward;
    private int rewardLimit = 60;

    [HideInInspector] public bool isPlay; //몬스터 이동 제어 등

    private List<Text> moneyTxtList = new List<Text>();

    private void Awake()
    {
        isPlay = true;

        if (Instance == null)
            Instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;
        playFabMgr = PlayFabManager.Instance;

        //_soulGem = (int)dataMgr.GameData.soulGem;
        //_gold = dataMgr.GameData.gold;
    }

    private void Update()
    {
        CancelBtn();
    }

    public void OnApplicationStart()
    {
        if (dataMgr == null)
            return;

        if (dataMgr.GameData.isNew == false)
        {
            StartCoroutine(WebChk());
            UIManager.Instance.SetActiveOfflineRewardNotice(true);
        }
        else
        {
            dataMgr.GameData.isNew = false;
        }
    }

    public void OnClickGetGold()
    {
        DataManager.Instance.GetGold();
    }

    #region 오프라인 시간
    private IEnumerator WebChk()
    {
        UnityWebRequest request = new UnityWebRequest();

        using (request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                //string date = request.GetResponseHeader("date");
                string offTime = dataMgr.GameData.lastLogInTimeStr;
                DateTime exitTime = Convert.ToDateTime(offTime);

                //DateTime dateTime = DateTime.Parse(date).ToUniversalTime();
                DateTime dateTime = DateTime.Now;
                TimeSpan timeStamp = dateTime - exitTime;

                stopWatch = (int)timeStamp.TotalSeconds;

                int minute = stopWatch / 60;
                int hour = minute / 60;
                int second = stopWatch % 60;
                minute = minute % 60;

                ShowOffReward(hour, minute, second);
            }
        }
    }

    public void ShowOffReward(int _hour, int _minute, int _second)
    {
        string infoTxt = "";

        if(_minute >= 10)
        {
            for (int i = 0; i < dataMgr.HumalData.humalList.Count; i++)
            {
                reward = (((dataMgr.HumalData.humalList[i].ID + 1)
                    * dataMgr.HumalData.humalList[i].Level * stopWatch)) / rewardLimit;
            }

            infoTxt = "자동 파밍 시간 : " + _hour.ToString() + "시간 "
                + _minute.ToString() + "분 " + _second.ToString() + "초";
        }
        else
        {
            infoTxt = "자동 파밍 시간 : 10분도 안 지났다구";
        }

        string rewardNumTxt = string.Format("{0:n0}", reward);
        UIManager.Instance.SetOfflineRewardUI(infoTxt, rewardNumTxt);
    }

    public void GetOffReward()
    {
        //dataMgr.GameData.soulGem += reward;

        SoundManager.Instance.PlaySFX("Button");
        UIManager.Instance.SetActiveOfflineRewardNotice(false);
    }
    #endregion

    public void OnClickGameStartBtn(StageInfo info)
    {
        if(dataMgr.HumalData.partyList.Count <= 0)
        {
            PopUpManager.Instance.PopUp("파티 정보가 없습니다!", EPopUpType.Caution);
            return;
        }

        //var _stageName = dataMgr.GameData.stageNameDic[_stageNum];
        dataMgr.SetStageInfo(info);
        SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Single);
    }

/*    public void ShowMoneyTxt()
    {
        //재화 텍스트 애니메이션 효과
        float dia = Mathf.SmoothStep(_dia,
            dataMgr.GameData.GoodsList[(int)EGoodsType.Dia].count, 0.5f);
        float gold = Mathf.SmoothStep(_gold,
            dataMgr.GameData.GoodsList[(int)EGoodsType.Gold].count, 0.5f);
        float drink = Mathf.SmoothStep(_drink,
            dataMgr.GameData.GoodsList[(int)EGoodsType.Stamina].count, 0.5f);
        //float soulGem = Mathf.SmoothStep(_soulGem, dataMgr.GameData.soulGem, 0.5f);

        _dia = (int)dia;
        _gold = (int)gold;
        _drink = (int)drink;

        //천 단위로 콤마(,) 삽입
*//*        moneyTxt[0].text = string.Format("{0:n0}",
            dataMgr.GameData.goodsList[(int)EGoodsType.Stamina].count);
        moneyTxt[1].text = string.Format("{0:n0}",
            dataMgr.GameData.goodsList[(int)EGoodsType.Gold].count);
        moneyTxt[2].text = string.Format("{0:n0}",
            dataMgr.GameData.goodsList[(int)EGoodsType.Dia].count);*//*
    }*/

    private void CancelBtn() //인 게임 Cancel 동작
    {
        if (UIManager.Instance.IsPanel)
            isPlay = false;

        if (Input.GetButtonDown("Cancel"))
            UIManager.Instance.SetActivePauseUI();
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}