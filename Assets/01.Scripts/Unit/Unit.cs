using System;
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

    //public UnitStatus myStat { get; protected set; }
    [SerializeField] protected UnitStatus mMyStat;
    public UnitStatus myStat { get => mMyStat; }

    [SerializeField] private GameObject dust;
    [SerializeField] private GameObject shadow;

    [SerializeField] private GameObject target;

    [SerializeField] private UnitJob mJob;
    public UnitJob job { get => mJob; }

    protected QueueType myObjType;

    protected int direction;

    protected float distance;
    protected string allyValue;
    protected string targetValue;

    protected bool isDust;
    protected bool isMove;

    protected float attackTime = 0;

    private bool IsAlive => myStat.hp > 0;

    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;

    protected abstract void CustomUnitSetup(UnitStatus status);

    public void UnitSetup(UnitStatus status)
    {
        CustomUnitSetup(status);
        //ChangeAc();
    }

    public virtual void ChangeAc()
    {
        anim.runtimeAnimatorController =
            DataManager.Instance.heroData.heroAnimCtrlList[myStat.ID];
    }

    protected void TeamValueSet()
    {
        direction = (this.gameObject.layer == 9) ? 1 : -1; //Team Value

        allyValue = (direction == 1) ? "Hero" : "Enemy"; //Layer Mask - Check Ally
        targetValue = (direction == 1) ? "Enemy" : "Hero"; //Layer Mask - Check Enemy

        switch (mJob) //Set Distance
        {
            case UnitJob.ShortRange: distance = 0.7f; myStat.ap = 2; myStat.hp = 10; break;
            case UnitJob.LongRange: distance = 2.5f; myStat.ap = 1; myStat.hp = 5; break;
            case UnitJob.Bullet: distance = 0.4f; break;
        }
    }

    protected virtual void Move()
    {
        if (battleMgr != null && !battleMgr.isPlay)
            return;

        if (isMove)
        {
            transform.Translate(new Vector2(direction * Time.deltaTime, 0));

            if (dust != null && isDust)
            {
                isDust = false;
                //anim.SetTrigger("doMove");
                dust.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    protected void Stop()
    {
        isDust = true;
        isMove = false;
        //anim.SetTrigger("doStop");
        //anim.ResetTrigger("doMove");

        if(dust != null)
            dust.GetComponent<ParticleSystem>().Stop();
    }

    protected void Attack()
    {
        float delay = UnityEngine.Random.Range(0.8f, 1.2f);

        if (attackTime >= delay)
        {
            attackTime = 0;
            //anim.SetTrigger("doAttack");

            switch (job)
            {
                case UnitJob.ShortRange: target.GetComponent<Unit>().Hit(myStat.ap); break;
                case UnitJob.LongRange: Shot(); break;
                //case UnitJob.Wizard: Spell(target.transform.position); break;
            }

            target.GetComponent<Unit>().Hit(myStat.ap);
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
        if (battleMgr != null && !battleMgr.isPlay)
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

    public virtual void Hit(int _atk)
    {
        if(IsAlive)
        {
            myStat.hp -= _atk;
            //anim.SetTrigger("doHit");
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (DeathAction != null)
            DeathAction();
    }
}