using System;
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
    [SerializeField] private BackendManager backendMgr;
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

        backendMgr = BackendManager.Instance;

        //_soulGem = (int)dataMgr.gameData.soulGem;
        //_gold = dataMgr.gameData.gold;
    }

    private void Update()
    {
        CancelBtn();
    }

    private void LateUpdate()
    {
        ShowMoneyTxt();
    }

    public void OnApplicationStart()
    {
        if (dataMgr == null)
            return;

        if (dataMgr.gameData.isNew == false)
        {
            StartCoroutine(WebChk());
            UIManager.Instance.SetActiveOfflineRewardNotice(true);
        }
        else
        {
            dataMgr.gameData.isNew = false;
        }
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
                string date = request.GetResponseHeader("date");
                string offTime = dataMgr.gameData.saveTimeStr;
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
            for (int i = 0; i < dataMgr.HeroData.heroList.Count; i++)
            {
                reward = (((dataMgr.HeroData.heroList[i].ID + 1)
                    * dataMgr.HeroData.heroList[i].level * stopWatch)) / rewardLimit;
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
        dataMgr.gameData.soulGem += reward;

        SoundManager.Instance.PlaySFX("Button");
        UIManager.Instance.SetActiveOfflineRewardNotice(false);
    }
    #endregion

    public void OnClickGameStartBtn(StageInfo info)
    {
        if(dataMgr.HeroData.partyList.Count <= 0)
        {
            NoticeManager.Instance.Notice(NoticeType.NotParty);
            return;
        }

        //var _stageName = dataMgr.gameData.stageNameDic[_stageNum];
        dataMgr.SetStageInfo(info);
        SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Single);
    }

    public void ShowMoneyTxt()
    {
        //재화 텍스트 애니메이션 효과
        float dia = Mathf.SmoothStep(_dia,
            dataMgr.gameData.goodsList[(int)GoodsType.Dia].count, 0.5f);
        float gold = Mathf.SmoothStep(_gold,
            dataMgr.gameData.goodsList[(int)GoodsType.Gold].count, 0.5f);
        float drink = Mathf.SmoothStep(_drink,
            dataMgr.gameData.goodsList[(int)GoodsType.Stamina].count, 0.5f);
        //float soulGem = Mathf.SmoothStep(_soulGem, dataMgr.gameData.soulGem, 0.5f);

        _dia = (int)dia;
        _gold = (int)gold;
        _drink = (int)drink;

        //천 단위로 콤마(,) 삽입
/*        moneyTxt[0].text = string.Format("{0:n0}",
            dataMgr.gameData.goodsList[(int)GoodsType.Stamina].count);
        moneyTxt[1].text = string.Format("{0:n0}",
            dataMgr.gameData.goodsList[(int)GoodsType.Gold].count);
        moneyTxt[2].text = string.Format("{0:n0}",
            dataMgr.gameData.goodsList[(int)GoodsType.Dia].count);*/
    }

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