﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UnitJob
{
    ShortRange = 0,
    LongRange,
    Bullet
}

public abstract class Unit : MonoBehaviour
{
    protected BattleManager battleMgr;

    public event Action DeathAction;

    [SerializeField] protected UnitData mMyData;
    public UnitData MyData { get => mMyData; }

    [SerializeField] protected int hp;
    public int Hp { get => hp; }

    [SerializeField] private GameObject dust;
    [SerializeField] private GameObject shadow;

    [SerializeField] private GameObject target;

    [SerializeField] private UnitJob mJob;
    public UnitJob Job { get => mJob; }

    protected QueueType myObjType;

    protected int direction;

    protected float distance;
    protected string allyValue;
    protected string targetValue;

    protected bool isDust;
    protected bool isMove;

    protected float attackTime = 0;

    private bool IsAlive => hp > 0;

    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;

    public void UnitSetup(UnitData unitData)
    {
        mMyData = unitData;
        sprite.color = Color.white;
        hp = unitData.hp;
        //ChangeAc();

        Debug.Log(name + " :: ap = " + unitData.ap);

        distance = unitData.distance;

        mMyData.mySprite = unitData.mySprite;
        sprite.sprite = mMyData.mySprite;

        mMyData.animCtrl = unitData.animCtrl;
        anim.runtimeAnimatorController = mMyData.animCtrl;
    }

    public virtual void ChangeAc()
    {
        anim.runtimeAnimatorController =
            DataManager.Instance.HeroData.heroAnimCtrlList[mMyData.ID];
    }

    protected void TeamValueSet()
    {
        direction = (this.gameObject.layer == 9) ? 1 : -1; //Team Value

        allyValue = (direction == 1) ? "Hero" : "Enemy"; //Layer Mask - Check Ally
        targetValue = (direction == 1) ? "Enemy" : "Hero"; //Layer Mask - Check Enemy
    }

    protected virtual void Move()
    {
        if (!IsAlive)
            return;

        if (battleMgr != null && !battleMgr.IsPlay)
            return;

        if (isMove)
        {
            transform.Translate(new Vector2(direction * Time.deltaTime, 0));

            if (dust != null && isDust)
            {
                isDust = false;
                anim.SetBool("isWalk", true);
                //anim.SetTrigger("doMove");
                dust.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    protected void Stop()
    {
        isDust = true;
        isMove = false;
        anim.SetBool("isWalk", false);
        //anim.SetTrigger("doStop");
        //anim.ResetTrigger("doMove");

        if (dust != null)
            dust.GetComponent<ParticleSystem>().Stop();
    }

    protected void Attack()
    {
        float delay = UnityEngine.Random.Range(0.8f, 1.2f);

        if (attackTime >= delay)
        {
            attackTime = 0;
            anim.SetTrigger("doAttack");

            switch (Job)
            {
                case UnitJob.ShortRange: target.GetComponent<Unit>().Hit(mMyData.ap); break;
                case UnitJob.LongRange: Shot(); break;
                //case UnitJob.Wizard: Spell(target.transform.position); break;
            }

            target.GetComponent<Unit>().Hit(mMyData.ap);
        }
        else
        {
            attackTime += Time.deltaTime;
        }
    }

    protected void Shot()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Arrow"),
            new Vector2(transform.position.x, transform.position.y + 0.3f), Quaternion.identity);
        bullet.layer = gameObject.layer;
        //bullet.gameObject.GetComponent<Bullet>().atk = atk;
        //bullet.gameObject.GetComponent<Bullet>().direction = direction;
    }

    protected void Spell(Vector2 pos)
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Magic"), pos, Quaternion.identity);
        bullet.layer = gameObject.layer;
    }

    protected void Scan()
    {
        if (!IsAlive)
            return;

        if (battleMgr != null && !battleMgr.IsPlay)
            return;

        RaycastHit2D rayHitAlly = Physics2D.Raycast(new Vector2(transform.position.x + 0.4f * direction, transform.position.y),
            Vector2.right * direction, 0.2f, LayerMask.GetMask(allyValue));

#if UNITY_EDITOR
        Debug.DrawRay(new Vector2(transform.position.x + 0.4f * direction, transform.position.y), Vector2.right * direction);
#endif

        RaycastHit2D rayHitEnemy = Physics2D.Raycast(transform.position,
            Vector2.right * direction, distance, LayerMask.GetMask(targetValue));

        if (rayHitEnemy.collider != null)
        {
            target = rayHitEnemy.collider.gameObject;

            Stop();
            Attack();
        }
        else
        {
            target = null;

            if (rayHitAlly.collider != null && rayHitAlly.collider.gameObject != gameObject
                && rayHitAlly.collider.CompareTag(allyValue))
                Stop();
            else
                isMove = true;
        }
    }

    protected virtual void Hit(int _atk)
    {
        if (!IsAlive)
            return;

        hp -= _atk;
        //anim.SetTrigger("doHit");

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        anim.SetTrigger("doDie");
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        sprite.color = Color.gray;
        Color alpha = sprite.color;

        float time = 0f;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / 1f;
            alpha.a = Mathf.Lerp(1, 0, time);
            sprite.color = alpha;
            yield return null;
        }

        if (DeathAction != null)
            DeathAction();

        yield return null;
    }
}