using System;
using System.Numerics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    private string GameDataFileName = "/GameData.json";
    private string MonsterDataFileName = "/MonsterData.json";

    #region 인스턴스화
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }

    public static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "DataManager";
                _instance = _container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(_container);
            }

            return _instance;
        }
    }

    public MonsData _monsData;
    public MonsData monsData
    {
        get
        {
            if (_monsData == null)
            {
                LoadMonsterData();
                SaveMonsterData();
            }

            return _monsData;
        }
    }

    public GameData _gameData;
    public GameData gameData
    {
        get
        {
            if(_gameData == null)
            {
                LoadGameData();
                SaveGameData();
            }

            return _gameData;
        }
    }
    #endregion

    private void Awake()
    {
       if(_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        LoadGameData();
    }

    private void Start()
    {
        LoadMonsterData();

        SaveGameData();
        SaveMonsterData();
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

    private void InitMonsterData()
    {
        for (int i = 0; i < 12; i++)
            monsData.monsUnlockList[i] = false;

        monsData.monsIndex = 0;
        monsData.monsDic.Clear();
        monsData.monsList.Clear();
    }

    public void ConstructMonsDic()
    {
        monsData.monsDic = new Dictionary<string, MonsterStatus>();

        //Json에 저장된 monsList안의 내용들을 monsDic에 저장
        for (int i = 0; i < monsData.monsList.Count; i++)
            monsData.monsDic.Add(monsData.monsList[i].name, monsData.monsList[i]);
    }

    public void ResettingMonsList()
    {
        monsData.monsList.Clear();

        for (int i = 0; i < monsData.monsDic.Count; i++)
            monsData.monsList = new List<MonsterStatus>(monsData.monsDic.Values);
    }

    #region Moster Load & Save
    public void LoadMonsterData()
    {
        string filePath = Application.persistentDataPath + MonsterDataFileName;

        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(code);
            string FromJsonData = System.Text.Encoding.UTF8.GetString(bytes);
            _monsData = JsonUtility.FromJson<MonsData>(FromJsonData);

            ConstructMonsDic();

            for (int i = 0; i < monsData.monsList.Count; i++)
            {
                GameObject mons = Instantiate(Resources.Load<GameObject>("Prefabs/Monster"), new UnityEngine.Vector3(0, 0, 0), UnityEngine.Quaternion.identity);
                mons.name = monsData.monsList[i].name;
                mons.GetComponent<Monster>().mID = monsData.monsList[i].ID;
                mons.GetComponent<Monster>().mExp = monsData.monsList[i].Exp;
                mons.GetComponent<Monster>().mLevel = monsData.monsList[i].Level;
                mons.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.monsSpriteList[monsData.monsList[i].ID];
                mons.GetComponent<Monster>().ChangeAc();
            }

            for (int i = 0; i < monsData.monsList.Count; i++)
            {
                //만약 monsDic에 해당 monsList의 "Jelly0"가 있다면
                bool found = monsData.monsDic.ContainsKey(monsData.monsList[i].name);

                if(found == false)
                    monsData.monsDic.Add(monsData.monsList[i].name, monsData.monsList[i]);
            }
        }
        else
        {
            _monsData = new MonsData();
            File.Create(Application.persistentDataPath + MonsterDataFileName);

            InitMonsterData();
        }
    }

    public void SaveMonsterData()
    {
        string filePath = Application.persistentDataPath + MonsterDataFileName;
        string ToJsonData = JsonUtility.ToJson(monsData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = System.Convert.ToBase64String(bytes);
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
            byte[] bytes = System.Convert.FromBase64String(code);
            string FromJsonData = System.Text.Encoding.UTF8.GetString(bytes);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }
        else
        {
            _gameData = new GameData();
            File.Create(Application.persistentDataPath + GameDataFileName);

            InitGameData();
        }
    }

    public void SaveGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        string ToJsonData = JsonUtility.ToJson(gameData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ToJsonData);
        string code = System.Convert.ToBase64String(bytes);
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
        SaveMonsterData();
    }
}