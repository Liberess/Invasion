using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using GooglePlayGames;

public class DataManager : MonoBehaviour
{
    [Header("==== Hero Object Prefab ====")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject lobbyHeroPrefab;

    public UnitStatus LeaderHero { get; private set; }

    #region 인스턴스화
    public static DataManager Instance { get; private set; }

    [Header("==== Game Data Information ===="), Space(10)]
    [SerializeField] private GameData m_GameData;
    public GameData GameData { get => m_GameData; }

    [Header("==== Hero Data Information ===="), Space(10)]
    [SerializeField] private HeroData m_HeroData;
    public HeroData HeroData { get => m_HeroData; }

    [Header("==== Enemy Data Information ===="), Space(10)]
    [SerializeField] private List<EnemyData> mEnemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList { get => mEnemyDataList; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        StartCoroutine(UpdateResources());
    }

    private void Start()
    {
        GameManager.Instance.OnApplicationStart();
    }

    public void SetStageInfo(StageInfo info) => GameData.stageInfo = info;

    #region Update Resources
    [ContextMenu("Update Resources")]
    public IEnumerator UpdateResources()
    {
        UpdateGoodsSprite();
        UpdateHeroSprite();
        UpdateOriginHeroData();
        UpdateHeroAnimCtrl();
        UpdateHeroCardSprite();
        UpdateEnemyData();
        yield return null;
    }

    private void UpdateGoodsSprite()
    {
        GameData.goodsSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("Goods");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                GameData.goodsSpriteList.Add(temp[i]);
        }
    }

    private void UpdateOriginHeroData()
    {
        HeroData.originHeroDataList.Clear();

        EnemyData[] temp = Resources.LoadAll<EnemyData>("Scriptable/OriginHeroData");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.originHeroDataList.Add(temp[i].myStat);
        }
    }

    //[ContextMenu("Update Hero Sprite")]
    private void UpdateHeroSprite()
    {
        HeroData.heroSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("HeroSprite");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroSpriteList.Add(temp[i]);
        }
    }

    //[ContextMenu("Update Hero Card Sprite")]
    private void UpdateHeroCardSprite()
    {
        HeroData.heroCardSpriteList.Clear();

        Sprite[] temp = Resources.LoadAll<Sprite>("HeroCardSprite");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroCardSpriteList.Add(temp[i]);
        }
    }

    //[ContextMenu("Update Hero Anim Controller")]
    private void UpdateHeroAnimCtrl()
    {
        HeroData.heroAnimCtrlList.Clear();

        RuntimeAnimatorController[] temp =
            Resources.LoadAll<RuntimeAnimatorController>("HeroAnimCtrl");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                HeroData.heroAnimCtrlList.Add(temp[i]);
        }
    }

    //[ContextMenu("Update Enemy Data")]
    private void UpdateEnemyData()
    {
        mEnemyDataList.Clear();

        EnemyData[] temp = Resources.LoadAll<EnemyData>("Scriptable/EnemyData");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                mEnemyDataList.Add(temp[i]);
        }
    }
    #endregion

    #region Hero List - Dic
    public void ConstructHeroDic()
    {
        m_HeroData.heroDic = new Dictionary<string, UnitStatus>();

        //Json에 저장된 heroList안의 내용들을 heroDic에 저장
        for (int i = 0; i < m_HeroData.heroList.Count; i++)
            m_HeroData.heroDic.Add(m_HeroData.heroList[i].name, m_HeroData.heroList[i]);
    }

    public void ResettingHeroList()
    {
        m_HeroData.heroList.Clear();

        for (int i = 0; i < m_HeroData.heroDic.Count; i++)
            m_HeroData.heroList = new List<UnitStatus>(m_HeroData.heroDic.Values);
    }

    /// <summary>
    /// PartyList에 id가 존재하는지 확인한다.
    /// </summary>
    public bool IsContainsInParty(int id)
    {
        foreach(var HeroData in m_HeroData.partyList)
        {
            if (HeroData.ID == id)
                return true;
        }

        return false;
    }

    /// <summary>
    /// index가 유효한 값인지 판별한다.
    /// </summary>
    /// <returns></returns>
    public bool IsValidInHeroListByIndex(int index)
    {
        if (index < 0 || index >= m_HeroData.heroList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// PartyList에서 hero가 몇 번째에 존재하는지 확인한다.
    /// </summary>
    public int GetIndexOfHeroInParty(UnitStatus data)
    {
        return Utility.FindIndexOf(m_HeroData.partyList, data);
    }

    public int GetIndexOfHeroInList(UnitStatus data)
    {
        return Utility.FindIndexOf(m_HeroData.heroList, data);
    }

    public UnitStatus GetDataByHeroID(int id)
    {
        foreach(var hero in m_HeroData.heroList)
        {
            if (hero.ID == id)
                return hero;
        }

        throw new Exception("ID : " + id + "는(은) 존재하지 않는 ID입니다.");
    }

    public UnitStatus GetDataByOrder(string key, int index)
    {
        if (!IsValidInHeroListByIndex(index))
            throw new Exception("유효하지 않은 Index 값입니다.");

        if(key == "Next")
        {
            if(!IsValidInHeroListByIndex(index + 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index + 1);
            return m_HeroData.heroList[index + 1];
        }
        else if(key == "Previous")
        {
            if (!IsValidInHeroListByIndex(index - 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index - 1);
            return m_HeroData.heroList[index - 1];
        }
        else
        {
            throw new Exception("유효하지 않은 Key 값입니다.");
        }
    }    

    public void SwapPartyData(int from, int to)
    {
        Utility.SwapListElement(m_HeroData.partyList, from, to);
    }

    public void UpdatePartyLeader()
    {
        for (int i = 0; i < m_HeroData.partyList.Count; i++)
        {
            if (i == 0)
            {
                m_HeroData.partyList[i].isLeader = true;
                LeaderHero = m_HeroData.partyList[i];
            }
            else
            {
                m_HeroData.partyList[i].isLeader = false;
            }
        }
    }
    #endregion

    public void GetGold()
    {
        try
        {
            SetGoods(EGoodsType.Gold, 1000);
            //GameData.goodsList[(int)EGoodsType.Gold].count += 1000;
        }
        catch(Exception exp)
        {
            Debug.Log(exp);
        }
    }

    public void SetGoods(EGoodsType EGoodsType, int num)
    {
        if (m_GameData.GoodsList.Count <= 0)
            throw new Exception("goodsList is empty");

        var count = m_GameData.GoodsList[(int)EGoodsType].count;
        if (count + num > int.MaxValue)
            throw new Exception("Overflow Max Goods Value");
        else if (count + num < 0)
            throw new Exception("Insufficient Goods Value");
        else
            m_GameData.AddElementInGoodsList((int)EGoodsType, num);
            //m_GameData.GoodsList[(int)EGoodsType].count += num;
    }

    public void InitializedData()
    {
        InitializedGameData();
        InitializedHeroData();
    }

    public IEnumerator LoadDataCo()
    {
        yield return LoadHeroDataCo();
        yield return LoadGameDataCo();
    }

    #region Load & Save
    private void InitializedHeroData()
    {
        for (int i = 0; i < HeroData.HeroMaxSize; i++)
            m_HeroData.heroUnlockList[i] = false;

        m_HeroData.heroDic.Clear();
        m_HeroData.heroList.Clear();
        m_HeroData.partyList.Clear();

        if (m_HeroData.originHeroDataList.Count <= 0)
            UpdateOriginHeroData();

        m_HeroData.heroUnlockList[0] = true;
        m_HeroData.heroList.Add(m_HeroData.originHeroDataList[0]);

        m_HeroData.isLoadComplete = true;
    }

    private void InitializedGameData()
    {
        string[] names = { "Stamina", "Gold", "Dia", "Awake Jewel" };
        m_GameData.goodsNames.Initialize();
        m_GameData.goodsNames = names;

        m_GameData.GoodsList.Clear();
        m_GameData.GoodsList = new List<Goods>();
        for (int i = 0; i < GameData.goodsNames.Length; i++)
        {
            m_GameData.GoodsList.Add(new Goods(GameData.goodsNames[i], 0));
            m_GameData.SetElementInGoodsList(i, 0);
        }

        for (int i = 0; i < m_GameData.facilGold.Length; i++)
            m_GameData.facilGold[i] = 0;

        m_GameData.sfx = 1f;
        m_GameData.bgm = 1f;

        m_GameData.isNew = true;

        m_GameData.saveTime = DateTime.Now;

        m_GameData.isLoadComplete = true;
    }

    public IEnumerator LoadHeroDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                string key = eachData.Key;

                if (eachData.Key.Contains("HeroData"))
                {
                    m_HeroData = JsonUtility.FromJson<HeroData>(eachData.Value.Value);
                    m_HeroData.isLoadComplete = true;

                    for (int i = 0; i < m_HeroData.heroList.Count; i++)
                    {
                        m_HeroData.heroList[i].mySprite = m_HeroData.heroSpriteList[m_HeroData.heroList[i].ID];
                        m_HeroData.heroList[i].animCtrl = m_HeroData.heroAnimCtrlList[m_HeroData.heroList[i].ID];
                    }

                    for (int i = 0; i < m_HeroData.partyList.Count; i++)
                    {
                        m_HeroData.partyList[i].mySprite = m_HeroData.heroSpriteList[m_HeroData.partyList[i].ID];
                        m_HeroData.partyList[i].animCtrl = m_HeroData.heroAnimCtrlList[m_HeroData.partyList[i].ID];
                    }

                    ConstructHeroDic();

                    for (int i = 0; i < m_HeroData.heroList.Count; i++)
                    {
                        //만약 heroDic에 해당 heroList의 "Jelly0"가 있다면
                        var targetName = m_HeroData.heroList[i].name;

                        if (!m_HeroData.heroDic.ContainsKey(targetName))
                            m_HeroData.heroDic.Add(targetName, m_HeroData.heroList[i]);
                    }

                    if (SceneManager.GetActiveScene().name == "Lobby")
                    {
                        for (int i = 0; i < m_HeroData.heroList.Count; i++)
                        {
                            var hero = Instantiate(lobbyHeroPrefab, Vector3.zero,
                                Quaternion.identity).GetComponent<LobbyHero>();

                            var heroStat = m_HeroData.heroList[i];
                            //heroStat.mySprite = HeroData.heroSpriteList[heroStat.ID];
                            //heroStat.animCtrl = HeroData.heroAnimCtrlList[heroStat.ID];

                            hero.UnitSetup(heroStat);
                        }
                    }
                }
                else
                {
                    InitializedHeroData();
                }
            }
        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }
    
    public IEnumerator LoadGameDataCo()
    {
        var request = new GetUserDataRequest() { PlayFabId = PlayFabManager.Instance.PlayFabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                string key = eachData.Key;

                if (eachData.Key.Contains("GameData"))
                {
                    m_GameData = JsonUtility.FromJson<GameData>(eachData.Value.Value);
                    m_GameData.isLoadComplete = true;
                }
                else
                {
                    InitializedGameData();
                }
            }
        }, DisplayPlayfabError);

        yield return new WaitForEndOfFrame();
    }

    public void SaveData()
    {
        SaveTime();
        Dictionary<string, string> dataDic = new Dictionary<string, string>();
        dataDic.Add("GameData", JsonUtility.ToJson(m_GameData));
        dataDic.Add("HeroData", JsonUtility.ToJson(m_HeroData));
        SetData(dataDic);
    }

    private void SetData(Dictionary<string, string> dataDic)
    {
        var request = new UpdateUserDataRequest() { Data = dataDic, Permission = UserDataPermission.Public };

        try
        {
            PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("Update Data!");
            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void SaveTime()
    {
        m_GameData.saveTime = DateTime.Now;
        m_GameData.saveTimeStr = m_GameData.saveTime.ToString();
    }
    #endregion

    private void DisplayPlayfabError(PlayFabError error)
    {
        Debug.LogError("error : " + error.GenerateErrorReport());
    }

    private void OnApplicationQuit()
    {
        if(Social.localUser.authenticated)
        {
            SaveTime();
            SaveData();
        }
    }
}