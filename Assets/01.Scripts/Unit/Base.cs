using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Unit
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
    }

    protected override void CustomUnitSetup(UnitStatus status)
    {
        mMyStat = status;
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }
}