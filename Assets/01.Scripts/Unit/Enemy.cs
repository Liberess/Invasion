using System;
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
        anim.runtimeAnimatorController = mMyStat.animCtrl;
    }
}