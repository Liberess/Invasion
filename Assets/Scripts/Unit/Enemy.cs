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

    public void Enrage()
    {
        ap = Mathf.FloorToInt(mData.Ap * 1.5f);
        moveSpeed *= 1.5f;
    }
}