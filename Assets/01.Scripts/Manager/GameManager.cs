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
    private DataManager dataMgr;

    [SerializeField] private StageInfo mStageInfo;
    public StageInfo stageInfo { get => mStageInfo; }

    public GameObject OptionPanel;
    public GameObject clearImg;
    public GameObject offNotice;
    public Text[] moneyTxt;

    public string[] monsNameList;

    private string url = "www.naver.com";

    private int stopWatch;

    private int _dia;
    private int _gold;
    private int _drink;

    private int reward;
    private int rewardLimit = 60;

    public bool isPlay; //몬스터 이동 제어 등
    public bool isPanel; //Panel이 켜져 있는가

    #region 인스턴스화
    private static GameObject Container;

    private static GameManager mInstance;
    public static GameManager Instance 
    {
        get
        {
            if (!mInstance)
            {
                Container = new GameObject();
                Container.name = "GameManager";
                mInstance = Container.AddComponent(typeof(GameManager)) as GameManager;
                DontDestroyOnLoad(Container);
            }

            return mInstance;
        }
    }
    #endregion

    private void Awake()
    {
        isPlay = true;

        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (mInstance != this)
        {
            Destroy(gameObject);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;

        //_soulGem = (int)DataManager.Instance.gameData.soulGem;
        //_gold = DataManager.Instance.gameData.gold;

        GameClear();

        if(dataMgr.gameData.isNew == false)
        {
            StartCoroutine(WebChk());
            offNotice.gameObject.GetComponent<Animator>().SetTrigger("doShow");
        }
        else
        {
            dataMgr.gameData.isNew = false;
        }
    }

    private void Update()
    {
        CancelBtn();
    }

    private void LateUpdate()
    {
        ShowMoneyTxt();
    }

    #region 오프라인 시간
    IEnumerator WebChk()
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
                string date = request.GetResponseHeader("date");
                string offTime = DataManager.Instance.gameData.saveTimeStr;
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
        if(_minute >= 10)
        {
            for (int i = 0; i < dataMgr.heroData.heroList.Count; i++)
            {
                reward = (((dataMgr.heroData.heroList[i].ID + 1)
                    * dataMgr.heroData.heroList[i].level * stopWatch)) / rewardLimit;
            }

            offNotice.transform.Find("OffTxt").gameObject.GetComponent<Text>().text =
                "자동 파밍 시간 : " + _hour.ToString() + "시간 " + _minute.ToString() + "분 " + _second.ToString() + "초";
        }
        else
        {
            offNotice.transform.Find("OffTxt").gameObject.GetComponent<Text>().text
                = "자동 파밍 시간 : 10분도 안 지났다구";
        }

        offNotice.transform.Find("JelatinNumTxt").gameObject.GetComponent<Text>().text = string.Format("{0:n0}", reward);
    }

    public void GetOffReward()
    {
        dataMgr.gameData.soulGem += reward;

        SoundManager.Instance.PlaySFX("Button");
        offNotice.gameObject.GetComponent<Animator>().SetTrigger("doHide");
    }
    #endregion

    public void OnClickGameStartBtn(string _stageNum)
    {
        //var _stageName = DataManager.Instance.gameData.stageNameDic[_stageNum];
        string _stageName = "도심";
        SetStageInfo(new StageInfo(_stageName, _stageNum, StageLevel.Easy, 0, 1));
        SceneManager.LoadScene("Battle");
    }

    public void GameClear()
    {
        if (DataManager.Instance.gameData.isClear)
        {
            NoticeManager.instance.Notice("Clear");
            clearImg.SetActive(true);
        }
        else
        {
            NoticeManager.instance.Notice("Start");
        }
    }

    public void ShowMoneyTxt()
    {
        //재화 텍스트 애니메이션 효과
        float dia = Mathf.SmoothStep(_dia, DataManager.Instance.gameData.dia, 0.5f);
        float gold = Mathf.SmoothStep(_gold, DataManager.Instance.gameData.gold, 0.5f);
        float drink = Mathf.SmoothStep(_drink, DataManager.Instance.gameData.drink, 0.5f);
        //float soulGem = Mathf.SmoothStep(_soulGem, DataManager.Instance.gameData.soulGem, 0.5f);

        _dia = (int)dia;
        _gold = (int)gold;
        _drink = (int)drink;

        //천 단위로 콤마(,) 삽입
        moneyTxt[0].text = string.Format("{0:n0}", DataManager.Instance.gameData.drink);
        moneyTxt[1].text = string.Format("{0:n0}", DataManager.Instance.gameData.gold);
        moneyTxt[2].text = string.Format("{0:n0}", DataManager.Instance.gameData.dia);

        /*        string str = string.Format("{0:n0}", DataManager.Instance.gameData.soulGem);
                DataManager.Instance.gameData.soulUnit = str.Split(',');
                moneyTxt[0].text = DataManager.Instance.SoulGemUnitChange(DataManager.Instance.gameData.strSoulGem);
                moneyTxt[0].text = DataManager.Instance.SoulGemUnitChange(DataManager.Instance.gameData.strDrink);*/
    }

    private void SetStageInfo(StageInfo info) => mStageInfo = info;

    private void CancelBtn() //인 게임 Cancel 동작
    {
        if (isPanel)
        {
            isPlay = false;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (!OptionPanel.activeSelf)
        {
            OptionPanel.SetActive(true);
            SoundManager.Instance.PlaySFX("Pause In");

            if (isPanel)
            {
                UIManager.Instance.hidePanelAction();
            }
        }
        else
        {
            OptionPanel.SetActive(false);
            SoundManager.Instance.PlaySFX("Pause Out");
        }
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}