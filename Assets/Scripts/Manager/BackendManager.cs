using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Start()
    {
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            //초기화 성공 시 로직
        }
        else
        {
            //초기화 실패 시 로직
        }
    }
}