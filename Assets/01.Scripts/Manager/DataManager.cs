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

    [SerializeField] private GameObject heroPrefab;

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

    private HeroData mHeroData;
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

    private GameData mGameData;
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

    private void InitHeroData()
    {
        for (int i = 0; i < 12; i++)
            heroData.heroUnlockList[i] = false;

        heroData.heroIndex = 0;
        heroData.heroDic.Clear();
        heroData.heroList.Clear();
    }

    public void ConstructHeroDic()
    {
        heroData.heroDic = new Dictionary<string, UnitStatus>();

        //Json에 저장된 heroList안의 내용들을 heroDic에 저장
        for (int i = 0; i < heroData.heroList.Count; i++)
            heroData.heroDic.Add(heroData.heroList[i].name, heroData.heroList[i]);
    }

    public void ResettingHeroList()
    {
        heroData.heroList.Clear();

        for (int i = 0; i < heroData.heroDic.Count; i++)
            heroData.heroList = new List<UnitStatus>(heroData.heroDic.Values);
    }

    #region Moster Load & Save
    public void LoadHeroData()
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
                var hero = Instantiate(heroPrefab, Vector3.zero, Quaternion.identity).GetComponent<Hero>();

                var heroStat = new UnitStatus(heroData.heroList[i].name,
                    heroData.heroList[i].ID, heroData.heroList[i].exp, heroData.heroList[i].level);

                hero.UnitSetup(heroStat);
                hero.name = heroData.heroList[i].name;
                hero.GetComponent<SpriteRenderer>().sprite =
                    heroData.heroSpriteList[heroData.heroList[i].ID];
            }

            for (int i = 0; i < heroData.heroList.Count; i++)
            {
                //만약 heroDic에 해당 heroList의 "Jelly0"가 있다면
                bool found = heroData.heroDic.ContainsKey(heroData.heroList[i].name);

                if(found == false)
                    heroData.heroDic.Add(heroData.heroList[i].name, heroData.heroList[i]);
            }
        }
        else
        {
            mHeroData = new HeroData();
            File.Create(Application.persistentDataPath + HeroDataFileName);

            InitHeroData();
        }
    }

    public void SaveHeroData()
    {
        string filePath = Application.persistentDataPath + HeroDataFileName;
        string ToJsonData = JsonUtility.ToJson(heroData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }
    #endregion

    #region Game Load & Save
    public void LoadGameData()
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

    public void SaveGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        string ToJsonData = JsonUtility.ToJson(gameData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }

    public void SaveTime()
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