﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        battleMgr = BattleManager.Instance;

        TeamValueSet();
    }

    private void OnEnable()
    {
        isDust = true;
        isMove = true;
    }

    private void OnDisable()
    {
        isDust = false;
        isMove = false;
    }

    private void Update() => Move();

    private void FixedUpdate() => Scan();

    protected override void CustomUnitSetup(UnitStatus status)
    {
        mMyStat = status;
        hp = status.hp;
        anim.runtimeAnimatorController = mMyStat.animCtrl;
    }

    public void Enrage()
    {
        mMyStat.ap = Mathf.FloorToInt(mMyStat.ap * 1.5f);
        mMyStat.moveSpeed *= 1.5f;
    }
}