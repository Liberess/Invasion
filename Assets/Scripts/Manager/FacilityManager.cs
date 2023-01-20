using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityManager : MonoBehaviour
{
    #region 인스턴스화
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }

    public static FacilityManager mInstance;
    public static FacilityManager Instance
    {
        get
        {
            if (!mInstance)
            {
                _container = new GameObject();
                _container.name = "FacilityManager";
                mInstance = _container.AddComponent(typeof(FacilityManager)) as FacilityManager;
                DontDestroyOnLoad(_container);
            }

            return mInstance;
        }
    }
    #endregion

    public GameObject[] timeTxtList;
    public Sprite[] facilSpriteList;
    public string[] facilNameList;
    public int[] facilGoldList;

    public Slider[] facilSliders;
    public GameObject[] locks;

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
        else if (mInstance != this)
            Destroy(this.gameObject);
    }

    private void Update()
    {
        //Change();
    }

    public void Change()
    {
        for(int i = 0; i < DataManager.Instance.GameData.facilUnlockList.Length; i++)
        {
            if (DataManager.Instance.GameData.facilUnlockList[i] == false)
            {
                locks[i].SetActive(true);
            }
            else
            {
                locks[i].SetActive(false);
            }
        }
    }

    public void BuildFacility()
    {

    }
}