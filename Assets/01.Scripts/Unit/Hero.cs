﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    private void Awake()
    {
        isDust = true;
        isMove = true;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        battleMgr = BattleManager.Instance;
        TeamValueSet();
    }

    private void Update() => Move();

    private void FixedUpdate() => Scan();
}