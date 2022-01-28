using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    private DataManager dataMgr;

    [SerializeField] private Text stageTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text costTxt;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Image leaderImg;

    [SerializeField] private GameObject[] heroCards = new GameObject[4];

    private float playTime = 0f;
    [SerializeField] private int cost = 0;
    [SerializeField] private int maxCost = 10;
    private float leaderGauge = 0f;
    [SerializeField] private float maxLeaderGauge = 10f;

    public bool isPlay { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        cost = 0;
        leaderGauge = 0f;
        costSlider.maxValue = maxCost;

        dataMgr = DataManager.Instance;

        SetHeroCard();
        SetCostSlider();
        SetLeaderGaugeImg();

        StartCoroutine(StartCoru());
    }

    private void Update()
    {
        if (isPlay == false)
            return;

        // Set PlayTime
        playTime += Time.deltaTime;
        SetPlayTimeText();
    }

    private IEnumerator StartCoru()
    {
        yield return new WaitForSeconds(0.5f);

        // Start 텍스트 출력, 애니메이션

        yield return new WaitForSeconds(1.5f);

        // Start 텍스트 종료

        yield return new WaitForSeconds(0.3f);

        // 게임 시작

        isPlay = true;
        StartCoroutine(GetCostCoru());
        StartCoroutine(GetLeaderGaugeCoru());
    }

    private IEnumerator GetCostCoru()
    {
        while (isPlay)
        {
            yield return new WaitForSeconds(1.3f);

            if (cost < maxCost)
            {
                ++cost;
                SetCostSlider();
            }
        }
    }

    private IEnumerator GetLeaderGaugeCoru()
    {
        while (isPlay)
        {
            yield return new WaitForSeconds(5f);

            if (leaderGauge < maxLeaderGauge)
            {
                ++leaderGauge;
                SetLeaderGaugeImg();
            }
        }
    }

    private void SetHeroCard()
    {
        for (int i = 0; i < heroCards.Length; i++)
        {
            if (dataMgr.heroData.partyList.Count <= 0 || dataMgr.heroData.partyList[i] == null)
                continue;

            var heroID = dataMgr.heroData.partyList[i].myStat.ID;

            // Set Hero Card Sprite
            heroCards[i].GetComponentInChildren<Image>().sprite =
                dataMgr.heroData.heroSlotSpriteList[heroID];

            // Set Hero Cost Text
            heroCards[i].GetComponentInChildren<Text>().text =
                dataMgr.heroData.heroCostList[heroID].ToString();
        }
    }

    private void SetPlayTimeText()
    {
        int minute = (int)playTime / 60;
        int second = (int)playTime % 60;
        minute %= 60;

        timeTxt.text = string.Format("{0:D2}", minute) + ":" + string.Format("{0:D2}", second);
    }

    private void SetCostSlider()
    {
        costSlider.value = cost;
        costTxt.text = cost + "/" + 10;
    }

    private void SetLeaderGaugeImg() =>
        leaderImg.fillAmount = leaderGauge / maxLeaderGauge;

    public void OnClickPauseBtn()
    {
        isPlay = false;
    }
}