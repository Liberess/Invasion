using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;
    [SerializeField] private AssetReference starYellowImgRef;
    [SerializeField] private AssetReference starPurpleImgRef;

    public async UniTaskVoid UpdateHeroInfo(UnitData HumalData)
    {
        heroInfo.nameTxt.text = HumalData.KoName;
        heroInfo.levelTxt.text = string.Concat("Lv.", HumalData.Level);
        heroInfo.heroImg.sprite = HumalData.sprite;

        heroInfo.dpsTxt.text = string.Concat("전투력 : ", HumalData.DPS);
        heroInfo.hpTxt.text = HumalData.HP.ToString();
        heroInfo.criticalTxt.text = string.Concat(HumalData.Critical, "%");
        heroInfo.apTxt.text = HumalData.Ap.ToString();
        heroInfo.dodgeTxt.text = string.Concat(HumalData.Dodge, "%");
        heroInfo.dpTxt.text = HumalData.Dp.ToString();
        heroInfo.costTxt.text = HumalData.Cost.ToString();

        foreach (var img in heroInfo.gradeImgs)
        {
            img.sprite = null;
            img.enabled = false;
        }

        await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset(starYellowImgRef) != null);
        await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset(starPurpleImgRef) != null);

        for (int i = 0; i < HumalData.Grade; i++)
        {
            if (i < 5)
            {
                heroInfo.gradeImgs[i].enabled = true;
                heroInfo.gradeImgs[i].sprite = ResourcesManager.Instance.LoadAsset(starYellowImgRef) as Sprite;
            }
            else
            {
                heroInfo.gradeImgs[i-5].enabled = true;
                heroInfo.gradeImgs[i-5].sprite = ResourcesManager.Instance.LoadAsset(starPurpleImgRef) as Sprite;
            }
        }

        /*
        if(DataManager.Instance.TryGetHumalPieceAmount(HumalData.ID, out int amount))
            heroInfo.pieceTxt.text = amount.ToString();*/
    }
}