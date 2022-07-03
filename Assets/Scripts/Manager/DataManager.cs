using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    private const string GameDataFileName = "/GameData.json";
    private const string HeroDataFileName = "/HeroData.json";

    [Header("==== Hero Object Prefab ====")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject lobbyHeroPrefab;

    public UnitStatus LeaderHero { get; private set; }

    #region 인스턴스화
    public static DataManager Instance { get; private set; }

    [Header("==== Game Data Information ===="), Space(10)]
    [SerializeField] private GameData mGameData;
    public GameData gameData
    {
        get
        {
            if (mGameData == null)
            {
                LoadGameData();
                //SaveGameData();
            }

            return mGameData;
        }
    }

    [Header("==== Hero Data Information ===="), Space(10)]
    [SerializeField] private HeroData mHeroData;
    public HeroData HeroData
    {
        get
        {
            if (mHeroData == null)
            {
                LoadHeroData();
                UpdateResources();
                //SaveHeroData();
            }

            return mHeroData;
        }
    }

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

        UpdateResources();
        LoadGameData();
    }

    private void Start()
    {
        GameManager.Instance.OnApplicationStart();

        LoadHeroData();

        SaveGameData();
        SaveHeroData();
    }

    public void SetStageInfo(StageInfo info) => gameData.stageInfo = info;

    public void SetMapUI()
    {

    }

    #region Update Resources
    [ContextMenu("Update Resources")]
    public IEnumerator UpdateResources()
    {
        UpdateHeroSprite();
        UpdateHeroAnimCtrl();
        UpdateHeroCardSprite();
        UpdateEnemyData();
        yield return null;
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

        EnemyData[] temp = Resources.LoadAll<EnemyData>("Scriptable");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
                mEnemyDataList.Add(temp[i]);
        }
    }
    #endregion

    public void ConstructHeroDic()
    {
        HeroData.heroDic = new Dictionary<string, UnitStatus>();

        //Json에 저장된 heroList안의 내용들을 heroDic에 저장
        for (int i = 0; i < HeroData.heroList.Count; i++)
            HeroData.heroDic.Add(HeroData.heroList[i].name, HeroData.heroList[i]);
    }

    public void ResettingHeroList()
    {
        HeroData.heroList.Clear();

        for (int i = 0; i < HeroData.heroDic.Count; i++)
            HeroData.heroList = new List<UnitStatus>(HeroData.heroDic.Values);
    }

    /// <summary>
    /// PartyList에 id가 존재하는지 확인한다.
    /// </summary>
    public bool IsContainsInParty(int id)
    {
        foreach(var HeroData in HeroData.partyList)
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
        if (index < 0 || index >= HeroData.heroList.Count)
            return false;

        return true;
    }

    /// <summary>
    /// PartyList에서 hero가 몇 번째에 존재하는지 확인한다.
    /// </summary>
    public int GetIndexOfHeroInParty(UnitStatus data)
    {
        return Utility.FindIndexOf(HeroData.partyList, data);
    }

    public int GetIndexOfHeroInList(UnitStatus data)
    {
        return Utility.FindIndexOf(HeroData.heroList, data);
    }

    public UnitStatus GetDataByHeroID(int id)
    {
        foreach(var hero in HeroData.heroList)
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
            return HeroData.heroList[index + 1];
        }
        else if(key == "Previous")
        {
            if (!IsValidInHeroListByIndex(index - 1))
                throw new Exception("유효하지 않은 Index 값입니다.");

            UIManager.Instance.SetHeroIndex(index - 1);
            return HeroData.heroList[index - 1];
        }
        else
        {
            throw new Exception("유효하지 않은 Key 값입니다.");
        }
    }    

    public void SwapPartyData(int from, int to)
    {
        Utility.SwapListElement(HeroData.partyList, from, to);
    }

    public void UpdatePartyLeader()
    {
        for(int i = 0; i < HeroData.partyList.Count; i++)
        {
            if (i == 0)
            {
                HeroData.partyList[i].isLeader = true;
                LeaderHero = HeroData.partyList[i];
            }
            else
            {
                HeroData.partyList[i].isLeader = false;
            }
        }
    }

    #region Hero Load & Save
    private void InitHeroData()
    {
        for (int i = 0; i < HeroData.HeroMaxSize; i++)
            HeroData.heroUnlockList[i] = false;

        HeroData.heroDic.Clear();
        HeroData.heroList.Clear();
    }

    private void LoadHeroData()
    {
        string filePath = Application.persistentDataPath + HeroDataFileName;

        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(code);
            string FromJsonData = System.Text.Encoding.UTF8.GetString(bytes);
            mHeroData = JsonUtility.FromJson<HeroData>(FromJsonData);

            for (int i = 0; i < HeroData.heroList.Count; i++)
            {
                HeroData.heroList[i].mySprite = HeroData.heroSpriteList[HeroData.heroList[i].ID];
                HeroData.heroList[i].animCtrl = HeroData.heroAnimCtrlList[HeroData.heroList[i].ID];
            }

            for (int i = 0; i < HeroData.partyList.Count; i++)
            {
                HeroData.partyList[i].mySprite = HeroData.heroSpriteList[HeroData.partyList[i].ID];
                HeroData.partyList[i].animCtrl = HeroData.heroAnimCtrlList[HeroData.partyList[i].ID];
            }

            ConstructHeroDic();

            for (int i = 0; i < HeroData.heroList.Count; i++)
            {
                //만약 heroDic에 해당 heroList의 "Jelly0"가 있다면
                var targetName = HeroData.heroList[i].name;

                if (!HeroData.heroDic.ContainsKey(targetName))
                    HeroData.heroDic.Add(targetName, HeroData.heroList[i]);
            }

            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                for (int i = 0; i < HeroData.heroList.Count; i++)
                {
                    var hero = Instantiate(lobbyHeroPrefab, Vector3.zero,
                        Quaternion.identity).GetComponent<LobbyHero>();

                    var heroStat = HeroData.heroList[i];
                    //heroStat.mySprite = HeroData.heroSpriteList[heroStat.ID];
                    //heroStat.animCtrl = HeroData.heroAnimCtrlList[heroStat.ID];

                    hero.UnitSetup(heroStat);
                }
            }

            //var newStat = new UnitStatus("나나", 0, 0, 1);
            //HeroData.partyList.Add(newStat);
            //HeroData.heroList.Add(newStat);
        }
        else
        {
            mHeroData = new HeroData();
            File.Create(Application.persistentDataPath + HeroDataFileName);

            InitHeroData();
        }
    }

    private void SaveHeroData()
    {
        string filePath = Application.persistentDataPath + HeroDataFileName;
        string ToJsonData = JsonUtility.ToJson(HeroData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }
    #endregion

    #region Game Load & Save
    private void InitGameData()
    {
        for (int i = 0; i < gameData.goods.Length; i++)
        {
            gameData.goods[i].name = gameData.goodsNames[i];
            gameData.goods[i].count = 0;
        }

        gameData.soulGem = 0;

        for (int i = 0; i < gameData.facilGold.Length; i++)
            gameData.facilGold[i] = 0;

        gameData.sfx = 1f;
        gameData.bgm = 1f;

        gameData.isNew = true;
        gameData.isClear = false;

        gameData.saveTime = DateTime.Now;
    }

    private void LoadGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(code);
            string FromJsonData = System.Text.Encoding.UTF8.GetString(bytes);
            mGameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }
        else
        {
            mGameData = new GameData();
            File.Create(Application.persistentDataPath + GameDataFileName);

            InitGameData();
        }
    }

    private void SaveGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        string ToJsonData = JsonUtility.ToJson(gameData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }

    private void SaveTime()
    {
        gameData.saveTime = DateTime.Now;
        gameData.saveTimeStr = gameData.saveTime.ToString();
    }
    #endregion

    private void OnApplicationQuit()
    {
        SaveTime();
        SaveGameData();
        SaveHeroData();
    }
}