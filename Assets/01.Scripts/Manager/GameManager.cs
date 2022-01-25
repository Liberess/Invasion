using System;
using System.Text;
using System.IO;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    DataManager dataManager;
    
    public GameObject OptionPanel;
    public GameObject clearImg;
    public GameObject offNotice;
    public Text[] moneyTxt;

    public Sprite[] monsSpriteList;
    public string[] monsNameList;
    public int[] monsGoldList;
    public int[] monsSoulGemList;
    public int[] numGoldList;
    public int[] clickGoldList;

    private string url = "www.naver.com";

    private int stopWatch;

    private int reward;
    private int rewardLimit = 60;

    //private BigInteger _soulGem;  //Txt Anim 효과를 위한 것
    //private BigInteger _gold;     //이하동문

    public bool isPlay; //몬스터 이동 제어 등
    public bool isPanel; //Panel이 켜져 있는가

    #region 인스턴스화
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }

    public static GameManager _instance;
    public static GameManager Instance 
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "GameManager";
                _instance = _container.AddComponent(typeof(GameManager)) as GameManager;
                DontDestroyOnLoad(_container);
            }

            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        isPlay = true;

        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        dataManager = DataManager.Instance;

        //_soulGem = (int)DataManager.Instance.gameData.soulGem;
        //_gold = DataManager.Instance.gameData.gold;

        GameClear();

        if(dataManager.gameData.isNew == false)
        {
            StartCoroutine(WebChk());
            offNotice.gameObject.GetComponent<Animator>().SetTrigger("doShow");
        }
        else
        {
            dataManager.gameData.isNew = false;
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
            for (int i = 0; i < dataManager.monsData.monsList.Count; i++)
            {
                reward = (((dataManager.monsData.monsList[i].ID + 1)
                    * dataManager.monsData.monsList[i].Level * stopWatch)) / rewardLimit;
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
        dataManager.gameData.soulGem += reward;

        SoundManager.Instance.PlaySFX("Button");
        offNotice.gameObject.GetComponent<Animator>().SetTrigger("doHide");
    }
    #endregion

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
        /* float soulGem = Mathf.SmoothStep(_soulGem, DataManager.Instance.gameData.soulGem, 0.5f);
        float gold = Mathf.SmoothStep(_gold, DataManager.Instance.gameData.gold, 0.5f);

         _soulGem = (int)soulGem;
         _gold = (int)gold;

        //천 단위로 콤마(,) 삽입
        moneyTxt[0].text = string.Format("{0:n0}", DataManager.Instance.gameData.soulGem);
        moneyTxt[1].text = string.Format("{0:n0}", DataManager.Instance.gameData.gold);

        string str = string.Format("{0:n0}", DataManager.Instance.gameData.soulGem);
        DataManager.Instance.gameData.soulUnit = str.Split(','); */

        //moneyTxt[0].text = DataManager.Instance.SoulGemUnitChange(DataManager.Instance.gameData.strSoulGem);
        //moneyTxt[0].text = DataManager.Instance.SoulGemUnitChange(DataManager.Instance.gameData.strDrink);

        moneyTxt[0].text = string.Format("{0:n0}", DataManager.Instance.gameData.drink);
        moneyTxt[1].text = string.Format("{0:n0}", DataManager.Instance.gameData.gold);
        moneyTxt[2].text = string.Format("{0:n0}", DataManager.Instance.gameData.dia);
    }

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