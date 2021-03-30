using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    public static MonsterPanel _instance;
    GameManager gameManager;
    DataManager dataManager;

    public GameObject lockGroup;

    public Image monsterLockImg;
    public Image monsterUnlockImg;

    public Text monsterNameTxt;
    public Text monsterKarmaTxt;
    public Text monsterGoldTxt;
    public Text pageTxt;

    public int page;

    private void Awake()
    {
        page = 0;
        _instance = this;
    }

    private void Start()
    {
        dataManager = DataManager.Instance;
        gameManager = GameManager.Instance;

        Change();
    }

    private void Change()
    {
        if(dataManager.monsData.monsUnlockList[page] == false)
        {
            lockGroup.SetActive(true);
        }
        else if (dataManager.monsData.monsUnlockList[page] == true)
        {
            lockGroup.SetActive(false);
        }

        monsterLockImg.sprite = gameManager.monsSpriteList[page];
        monsterUnlockImg.sprite = gameManager.monsSpriteList[page];
        monsterLockImg.SetNativeSize();
        monsterUnlockImg.SetNativeSize();

        monsterNameTxt.text = gameManager.monsNameList[page];
        monsterGoldTxt.text = string.Format("{0:n0}", gameManager.monsGoldList[page]);
        monsterKarmaTxt.text = string.Format("{0:n0}", gameManager.monsSoulGemList[page]);
        pageTxt.text = string.Format("#{0:00}", page + 1);
    }

    public void Unlock()
    {
        if (dataManager.gameData.soulGem >= gameManager.monsSoulGemList[page])
        {
            dataManager.gameData.soulGem -= gameManager.monsSoulGemList[page];
            dataManager.monsData.monsUnlockList[page] = true;

            Change();

            SoundManager.Instance.PlaySFX("Unlock");

            for(int i = 0; i < dataManager.monsData.monsUnlockList.Length; i++)
            {
                if(dataManager.monsData.monsUnlockList[i] == true)
                {
                    dataManager.gameData.isClear = true;
                }
                else
                {
                    dataManager.gameData.isClear = false;
                }
            }

            if(dataManager.gameData.isClear)
            {
                gameManager.GameClear();
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX("Fail");
            NoticeManager.instance.Notice("NotJelatin");
        }
    }

    public void Buy()
    {
        if (dataManager.gameData.gold >= gameManager.monsGoldList[page])
        {
            if(dataManager.monsData.monsList.Count < (dataManager.gameData.numLevel * 2))
            {
                dataManager.gameData.gold -= gameManager.monsGoldList[page];

                GameObject mons = Instantiate(Resources.Load<GameObject>("Prefabs/Monster"), new Vector3(0, 0, 0), Quaternion.identity);
                mons.name = string.Concat("Jelly" + dataManager.monsData.monsIndex);
                mons.GetComponent<Monster>().mID = page;
                mons.GetComponent<SpriteRenderer>().sprite = gameManager.monsSpriteList[page];

                dataManager.monsData.monsDic.Add(mons.name, new MonsterStatus(mons.name, page, 0, 1));
                dataManager.monsData.monsList = new List<MonsterStatus>(dataManager.monsData.monsDic.Values);
                dataManager.monsData.monsIndex++;

                SoundManager.Instance.PlaySFX("Buy");
            }
            else
            {
                NoticeManager.instance.Notice("NotNum");
                SoundManager.Instance.PlaySFX("Fail");
            }
        }
        else
        {
            NoticeManager.instance.Notice("NotGold");
            SoundManager.Instance.PlaySFX("Fail");
        }
    }

    #region 페이지 컨트롤
    private void PageUp()
    {
        if (page != 11)
        {
            ++page;

            Change();

            SoundManager.Instance.PlaySFX("Button");
        }
    }

    private void PageDown()
    {
        if (page != 0)
        {
            --page;

            Change();

            SoundManager.Instance.PlaySFX("Button");
        }
    }
    #endregion
}