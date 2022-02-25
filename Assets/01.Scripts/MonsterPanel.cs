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
        if(dataManager.heroData.heroUnlockList[page] == false)
        {
            lockGroup.SetActive(true);
        }
        else if (dataManager.heroData.heroUnlockList[page] == true)
        {
            lockGroup.SetActive(false);
        }

        //monsterLockImg.sprite = dataManager.enemyData.enemySpriteList[page];
        //monsterUnlockImg.sprite = dataManager.enemyData.enemySpriteList[page];

        monsterLockImg.SetNativeSize();
        monsterUnlockImg.SetNativeSize();

        monsterNameTxt.text = gameManager.monsNameList[page];
        //monsterGoldTxt.text = string.Format("{0:n0}", gameManager.monsGoldList[page]);
        //monsterKarmaTxt.text = string.Format("{0:n0}", gameManager.heroSoulGemList[page]);
        pageTxt.text = string.Format("#{0:00}", page + 1);
    }

    public void Unlock()
    {
        if (dataManager.gameData.soulGem >= dataManager.heroData.heroSoulGemList[page])
        {
            dataManager.gameData.soulGem -= dataManager.heroData.heroSoulGemList[page];
            dataManager.heroData.heroUnlockList[page] = true;

            Change();

            SoundManager.Instance.PlaySFX("Unlock");

            for(int i = 0; i < dataManager.heroData.heroUnlockList.Length; i++)
            {
                if(dataManager.heroData.heroUnlockList[i] == true)
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
            NoticeManager.Instance.Notice(NoticeType.NotGold);
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