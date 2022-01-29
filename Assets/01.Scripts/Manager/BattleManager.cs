using System;
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

    [SerializeField] private GameObject heroCardGrid;
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private List<GameObject> heroCardList = new List<GameObject>();

    private float playTime = 0f;
    [SerializeField] private int cost = 0;
    [SerializeField] private int maxCost = 10;
    private float leaderGauge = 0f;
    [SerializeField] private float maxLeaderGauge = 10f;

    private Action UpdateCardAction;

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
        heroCardList.Clear();

        for (int i = 0; i < dataMgr.heroData.partyList.Count; i++)
        {
            var heroCard = Instantiate(heroCardPrefab, Vector3.zero, Quaternion.identity);
            heroCard.transform.SetParent(heroCardGrid.transform);
            heroCard.transform.localScale = Vector3.one;

            var heroID = dataMgr.heroData.partyList[i].ID;

            // Set Hero Card Sprite
            heroCard.transform.GetChild(0).GetComponent<Image>().sprite =
                dataMgr.heroData.heroSlotSpriteList[heroID];

            // Set Hero Cost Text
            heroCard.transform.GetChild(1).GetComponent<Text>().text =
                dataMgr.heroData.heroCostList[heroID].ToString();

            // Set Hero Card OnClick Event
            heroCard.GetComponent<Button>().onClick.AddListener(() => OnClickHeroCard(heroID));
            UpdateCardAction += () => UpdateCardEvent(heroCard, heroID);

            heroCardList.Add(heroCard);
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
        UpdateCardAction();
        costSlider.value = cost;
        costTxt.text = cost + "/" + 10;
    }

    private void SetLeaderGaugeImg() =>
        leaderImg.fillAmount = leaderGauge / maxLeaderGauge;

    public void OnClickPauseBtn()
    {
        isPlay = false;
    }

    private void UpdateCardEvent(GameObject target, int id)
    {
        var targetCost = dataMgr.heroData.heroCostList[id];

        var button = target.GetComponent<Button>();
        var lockImg = target.transform.GetChild(2).gameObject;

        if(cost >= targetCost)
        {
            button.interactable = true;
            lockImg.SetActive(false);
        }
        else
        {
            button.interactable = false;
            lockImg.SetActive(true);
        }
    }

    private void OnClickHeroCard(int id)
    {
        Debug.Log("OnClickHeroCard : " + id);
        var targetCost = dataMgr.heroData.heroCostList[id];

        if(cost >= targetCost)
        {
            cost -= targetCost;
            SetCostSlider();
        }
    }
}