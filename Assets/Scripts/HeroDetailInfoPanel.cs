using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private AssetReference starYellowImgRef;
    [SerializeField] private AssetReference starPurpleImgRef;
    
    [SerializeField] private HeroDetailInfo heroInfo;

    [SerializeField] private Slider gradeSlider;
    [SerializeField] private Image pieceImg;
    [SerializeField] private Text pieceTxt;

    [SerializeField] private GameObject levelGroup;
    [SerializeField] private Image awakeImg;
    [SerializeField] private Text awakeTxt;
    [SerializeField] private Image goldImg; 
    [SerializeField] private Text goldTxt;

    private UnitData currentHumalData;

    private void Awake()
    {
        gradeSlider.transform.parent.GetComponent<Button>().onClick.AddListener(OnClickUpgrade);
    }

    private void UpdatePanel()
    {
        if (currentHumalData != null)
        {
            if (currentHumalData.Level < 30) //Level
            {
                levelGroup.SetActive(true);
                gradeSlider.gameObject.SetActive(false);
                
                var entity = DataManager.Instance.HumalData.humalUpgradeLevelList.Find(x => x.lv == currentHumalData.Level);
                awakeTxt.text = Utility.ConcatCurrency(DataManager.Instance.GetCurrency(ECurrencyType.AJ), entity.require_awake_jewel);
                goldTxt.text = Utility.ConcatCurrency(DataManager.Instance.GetCurrency(ECurrencyType.GD), entity.require_gold);
                //awakeTxt.text = string.Concat(string.Format("{0:n0}", DataManager.Instance.GetCurrency(ECurrencyType.AJ)), "/", string.Format("{0:n0}", entity.require_awake_jewel));
                //goldTxt.text = string.Concat(DataManager.Instance.GetCurrency(ECurrencyType.GD), "/", entity.require_gold);
            }
            else
            {
                levelGroup.SetActive(false);
                gradeSlider.gameObject.SetActive(true);
                
                var entity = DataManager.Instance.HumalData.humalUpgradeGradeList.Find(x => x.lv == currentHumalData.Grade);
                if (DataManager.Instance.TryGetHumalPieceAmount(currentHumalData.ID, out int amount))
                {
                    gradeSlider.maxValue = entity.require_piece;
                    gradeSlider.value = amount;
                    pieceTxt.text = string.Concat(amount, "/", entity.require_piece);
                }
            }

            UIManager.Instance.UpdateHumalSlotDataByID(currentHumalData.ID);
            UIManager.Instance.UpdateHumalSlotByID(currentHumalData.ID);
        }
    }

    private void OnClickUpgrade()
    {
        var data = DataManager.Instance.GetHumalDataByID(currentHumalData.ID);

        if (data.Level < 30) //Level
        {
            var entity = DataManager.Instance.HumalData.humalUpgradeLevelList.Find(x => x.lv == data.Level);
            if (DataManager.Instance.SetCurrencyAmount(ECurrencyType.AJ, -entity.require_awake_jewel) &&
                DataManager.Instance.SetCurrencyAmount(ECurrencyType.GD, -entity.require_gold))
            {
                data.UpgradeLevel(1);
                currentHumalData = data;
                UpdatePanel();
                UpdateHeroInfo(data).Forget();
                PopUpManager.Instance.PopUp("레벨 강화 성공!", EPopUpType.Notice);
            }
            else
            {
                PopUpManager.Instance.PopUp("레벨 강화 실패!", EPopUpType.Warning); 
            }
        }
        else //Grade
        {
            var entity = DataManager.Instance.HumalData.humalUpgradeGradeList.Find(x => x.lv == data.Grade);
            if (DataManager.Instance.SubtractHumalPiece(data.ID, entity.require_piece))
            {
                data.UpgradeGrade(1);
                currentHumalData = data;
                UpdatePanel();
                UpdateHeroInfo(data).Forget();
                PopUpManager.Instance.PopUp("등급 강화 성공!", EPopUpType.Notice);
            }
            else
            {
                PopUpManager.Instance.PopUp("등급 강화 실패", EPopUpType.Warning);
            }
        }
    }

    public async UniTaskVoid UpdateHeroInfo(UnitData humalData)
    {
        currentHumalData = humalData;
        UpdatePanel();
        
        heroInfo.nameTxt.text = humalData.KoName;
        heroInfo.levelTxt.text = string.Concat("Lv.", humalData.Level);
        heroInfo.heroImg.sprite = humalData.sprite;

        heroInfo.dpsTxt.text = string.Concat("전투력 : ", humalData.DPS);
        heroInfo.hpTxt.text = humalData.HP.ToString();
        heroInfo.criticalTxt.text = string.Concat(humalData.Critical, "%");
        heroInfo.apTxt.text = humalData.Ap.ToString();
        heroInfo.dodgeTxt.text = string.Concat(humalData.Dodge, "%");
        heroInfo.dpTxt.text = humalData.Dp.ToString();
        heroInfo.costTxt.text = humalData.Cost.ToString();

        foreach (var img in heroInfo.gradeImgs)
        {
            img.sprite = null;
            img.enabled = false;
        }

        await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset<Sprite>(starYellowImgRef) != null);
        await UniTask.WaitUntil(() => ResourcesManager.Instance.LoadAsset<Sprite>(starPurpleImgRef) != null);

        for (int i = 0; i < humalData.Grade; i++)
        {
            if (i < 5)
            {
                heroInfo.gradeImgs[i].enabled = true;
                heroInfo.gradeImgs[i].sprite = ResourcesManager.Instance.LoadAsset<Sprite>(starYellowImgRef) as Sprite;
            }
            else
            {
                heroInfo.gradeImgs[i-5].enabled = true;
                heroInfo.gradeImgs[i-5].sprite = ResourcesManager.Instance.LoadAsset<Sprite>(starPurpleImgRef) as Sprite;
            }
        }

        /*
        if(DataManager.Instance.TryGetHumalPieceAmount(HumalData.ID, out int amount))
            heroInfo.pieceTxt.text = amount.ToString();*/
    }
}