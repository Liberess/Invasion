using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    [SerializeField] private HumalData humalData;
    public HumalData HumalData => humalData;

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

    public override void UnitSetup(UnitData unitData)
    {
        base.UnitSetup(unitData);

        mData = unitData;
        humalData.SetHumalData(unitData);
    }

    public void UnitSetup(HumalData humalData)
    {
        this.humalData = humalData;
    }
}