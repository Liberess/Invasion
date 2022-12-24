﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendInitializer : MonoBehaviour
{
    private void Awake()
    {
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log(Backend.Utils.GetGoogleHash());
            
  /*          var federationAuth = GetComponent<BackendFederationAuth>();
            if (federationAuth != null)
                federationAuth.SetupGPGS();*/
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