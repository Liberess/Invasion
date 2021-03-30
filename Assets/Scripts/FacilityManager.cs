using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityManager : MonoBehaviour
{
    public GameObject[] timeTxtList;
    public Sprite[] facilSpriteList;
    public string[] facilNameList;
    public int[] facilGoldList;

    public Slider[] facilSliders;
    public GameObject[] locks;

    #region 인스턴스화
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }

    public static FacilityManager _instance;
    public static FacilityManager Instance
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "FacilityManager";
                _instance = _container.AddComponent(typeof(FacilityManager)) as FacilityManager;
                DontDestroyOnLoad(_container);
            }

            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Change();
    }

    public void Change()
    {
        for(int i = 0; i < DataManager.Instance.gameData.facilUnlockList.Length; i++)
        {
            if (DataManager.Instance.gameData.facilUnlockList[i] == false)
            {
                locks[i].SetActive(true);
            }
            else
            {
                locks[i].SetActive(false);
            }
        }
    }
}