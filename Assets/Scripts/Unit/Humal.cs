﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humal : Unit
{
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        battleMgr = BattleManager.Instance;
        //spriteRen.flipX = true;
        TeamValueSet();
    }

    private void Update() => Move();

    private void FixedUpdate() => Scan();
}