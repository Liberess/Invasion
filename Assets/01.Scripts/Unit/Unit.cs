using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UnitType
{
    ShortRange= 0,
    LongRange,
    Bullet
}

public abstract class Unit : MonoBehaviour
{
    protected BattleManager battleMgr;

    public UnitStatus myStat { get; protected set; }

    [SerializeField] private GameObject dust;
    [SerializeField] private GameObject shadow;

    private GameObject enemy;

    [SerializeField] private UnitType mJob;
    public UnitType job { get => mJob; }

    protected int direction;

    protected float distance;
    protected string allyValue;
    protected string enemyValue;

    protected bool isDust;
    protected bool isMove;

    protected float attackTime = 0;

    private bool IsAlive => myStat.hp > 0;

    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;

    public void UnitSetup(UnitStatus status)
    {
        myStat = status;
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
        enemyValue = (direction == 1) ? "Enemy" : "Hero"; //Layer Mask - Check Enemy

        switch (mJob) //Set Distance
        {
            case UnitType.ShortRange: distance = 0.7f; myStat.ap = 2; myStat.hp = 10; break;
            case UnitType.LongRange: distance = 2.5f; myStat.ap = 1; myStat.hp = 5; break;
            case UnitType.Bullet: distance = 0.4f; break;
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
        float delay = Random.Range(0.8f, 1.2f);

        if (attackTime >= delay)
        {
            attackTime = 0;
            //anim.SetTrigger("doAttack");

            switch (job)
            {
                case UnitType.ShortRange: enemy.GetComponent<Unit>().Hit(myStat.ap); break;
                case UnitType.LongRange: Shot(); break;
                //case UnitType.Wizard: Spell(enemy.transform.position); break;
            }

            enemy.GetComponent<Unit>().Hit(myStat.ap);
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

        RaycastHit2D rayHitAlly = Physics2D.Raycast(transform.position,
            Vector2.right * direction, 0.2f, LayerMask.GetMask(allyValue));

        RaycastHit2D rayHitEnemy = Physics2D.Raycast(transform.position,
            Vector2.right * direction, distance, LayerMask.GetMask(enemyValue));

        if (rayHitEnemy.collider != null)
        {
            enemy = rayHitEnemy.collider.gameObject;

            Stop();
            Attack();
        }
        else
        {
            enemy = null;

            if (rayHitAlly.collider != null)
                Stop();
            else
                isMove = true;
        }
    }

    public void Hit(int _atk)
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

    protected void Die()
    {
        Destroy(gameObject);
    }
}