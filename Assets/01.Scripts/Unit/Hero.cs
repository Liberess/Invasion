using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
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
        sprite.flipX = true;
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
        
        mMyStat.mySprite = status.mySprite;
        sprite.sprite = mMyStat.mySprite;

        mMyStat.animCtrl = status.animCtrl;
        anim.runtimeAnimatorController = mMyStat.animCtrl;
/*        sprite.sprite = DataManager.Instance.heroData.heroSpriteList[status.ID];
        anim.runtimeAnimatorController =
            DataManager.Instance.heroData.heroAnimCtrlList[status.ID];*/
    }
}