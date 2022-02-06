using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    private const string GameDataFileName = "/GameData.json";
    private const string HeroDataFileName = "/HeroData.json";

    [Header("== Hero Object Prefab ==")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject lobbyHeroPrefab;

    #region 인스턴스화
    private static GameObject mContainer;

    private static DataManager mInstance;
    public static DataManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mContainer = new GameObject();
                mContainer.name = "DataManager";
                mInstance = mContainer.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(mContainer);
            }

            return mInstance;
        }
    }

    [Header("== Hero Data Information =="), Space(10)]
    [SerializeField] private HeroData mHeroData;
    public HeroData heroData
    {
        get
        {
            if (mHeroData == null)
            {
                LoadHeroData();
                SaveHeroData();
            }

            return mHeroData;
        }
    }

    [Header("== Game Data Information =="), Space(10)]
    [SerializeField] private GameData mGameData;
    public GameData gameData
    {
        get
        {
            if(mGameData == null)
            {
                LoadGameData();
                SaveGameData();
            }

            return mGameData;
        }
    }
    #endregion

    private void Awake()
    {
       if(mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(mInstance != this)
        {
            Destroy(gameObject);
        }

        LoadGameData();
    }

    private void Start()
    {
        LoadHeroData();

        SaveGameData();
        SaveHeroData();
    }

    #region Update Resources
    [ContextMenu("Update Hero Sprite")]
    private void UpdateHeroSprite()
    {
        heroData.heroSpriteList.Clear();

        object[] temp = Resources.LoadAll("HeroSprite");

        foreach (var obj in temp)
            heroData.heroSpriteList.Add(obj as Sprite);

        heroData.heroSpriteList.RemoveAt(0);
    }

    [ContextMenu("Update Hero Card Sprite")]
    private void UpdateHeroCardSprite()
    {
        heroData.heroCardSpriteList.Clear();

        object[] temp = Resources.LoadAll("HeroCardSprite");

        foreach (var obj in temp)
            heroData.heroCardSpriteList.Add(obj as Sprite);

        heroData.heroCardSpriteList.RemoveAt(0);
    }

    [ContextMenu("Update Hero Anim Controller")]
    private void UpdateHeroAnimCtrl()
    {
        heroData.heroAnimCtrlList.Clear();

        object[] temp = Resources.LoadAll("HeroAnimCtrl");

        foreach (var obj in temp)
            heroData.heroAnimCtrlList.Add(obj as RuntimeAnimatorController);

        //heroData.heroAnimCtrlList.RemoveAt(0);
    }
    #endregion

    public void ConstructHeroDic()
    {
        heroData.heroDic = new Dictionary<string, UnitStatus>();

        //Json에 저장된 heroList안의 내용들을 heroDic에 저장
        for (int i = 0; i < heroData.heroList.Count; i++)
            heroData.heroDic.Add(heroData.heroList[i].myName, heroData.heroList[i]);
    }

    public void ResettingHeroList()
    {
        heroData.heroList.Clear();

        for (int i = 0; i < heroData.heroDic.Count; i++)
            heroData.heroList = new List<UnitStatus>(heroData.heroDic.Values);
    }

    #region Moster Load & Save
    private void InitHeroData()
    {
        for (int i = 0; i < HeroData.HeroMaxSize; i++)
            heroData.heroUnlockList[i] = false;

        heroData.heroIndex = 0;
        heroData.heroDic.Clear();
        heroData.heroList.Clear();
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

            ConstructHeroDic();

            for (int i = 0; i < heroData.heroList.Count; i++)
            {
                var hero = Instantiate(lobbyHeroPrefab, Vector3.zero,
                    Quaternion.identity).GetComponent<LobbyHero>();

                var heroStat = heroData.heroList[i];
                //heroStat.mySprite = heroData.heroSpriteList[heroStat.ID];
                //heroStat.animCtrl = heroData.heroAnimCtrlList[heroStat.ID];

                hero.UnitSetup(heroStat);
            }

            for (int i = 0; i < heroData.heroList.Count; i++)
            {
                //만약 heroDic에 해당 heroList의 "Jelly0"가 있다면
                var targetName = heroData.heroList[i].myName;

                if(!heroData.heroDic.ContainsKey(targetName))
                    heroData.heroDic.Add(targetName, heroData.heroList[i]);
            }

            //var newStat = new UnitStatus("나나", 0, 0, 1);
            //heroData.partyList.Add(newStat);
            //heroData.heroList.Add(newStat);
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
        string ToJsonData = JsonUtility.ToJson(heroData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }
    #endregion

    #region Game Load & Save
    private void InitGameData()
    {
        gameData.dia = 0;
        gameData.gold = 0;
        gameData.soulGem = 0;
        gameData.drink = 0;

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

        if(File.Exists(filePath))
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