using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] private Text nameTxt;
    [SerializeField] private Text numTxt;
    [SerializeField] private Button startBtn;

    public void SetupStageInfo(StageInfo info)
    {
        nameTxt.text = info.stageName;
        numTxt.text = info.stageNum;

        startBtn.onClick.AddListener(() => GameManager.Instance.OnClickGameStartBtn(info));
    }
}