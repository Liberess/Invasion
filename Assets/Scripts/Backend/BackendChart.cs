using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;

public class BackendChart : MonoBehaviour
{
    public void OnClickGetChartAndSave()
    {
        BackendReturnObject BRO = Backend.Chart.GetOneChartAndSave("54545", "캐릭터_데이터");

        if (BRO.IsSuccess())
        {
            Debug.Log("불러오기 완료");
            Debug.Log(BRO);

            JsonData rows = BRO.GetReturnValuetoJSON()["rows"];
            string ChartName, ChartContents;

            for (int i = 0; i < rows.Count; i++)
            {
                ChartName = rows[i]["chartName"]["S"].ToString();

                ChartContents = PlayerPrefs.GetString(ChartName);
                Debug.Log(string.Format("{0}\n{1}", ChartName, ChartContents));

                GetPlayerPrefs(ChartName);
            }
        }
        else
        {
            Debug.Log("서버 공통 에러 발생 : " + BRO.GetMessage());
        }
    }

    private void GetPlayerPrefs(string chartName)
    {
        string chartString = PlayerPrefs.GetString(chartName);

        JsonData chartJson = JsonMapper.ToObject(chartString)["rows"][1];
        Debug.Log(chartJson["name"][0]);
    }
}