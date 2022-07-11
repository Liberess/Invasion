using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendInitializer : MonoBehaviour
{
    private void Start()
    {
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log("초기화 성공!");
        }
        else
        {
            Debug.Log("초기화 실패!");
        }
    }

    private void Update()
    {
        Backend.AsyncPoll();
    }
}