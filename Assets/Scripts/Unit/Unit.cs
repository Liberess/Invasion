using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : LivingEntity
{
    protected BattleManager battleMgr;

    [Space(5), Header("==== Unit Data ====")]
    [SerializeField] protected UnitData unitData;
    public UnitData UnitData => unitData;

    [Space(5), Header("==== Current Status ====")]
    [SerializeField] private EUnitJob mJob;
    public EUnitJob Job { get => mJob; }
    protected EUnitQueueType myObjType;

    [SerializeField] protected int ap;
    public int Ap { get => ap; }

    [SerializeField] protected float moveSpeed;
    public float MoveSpeed { get => moveSpeed; }

    [Space(5), Header("==== Particle ====")]
    [SerializeField] private GameObject dust;
    [SerializeField] private GameObject shadow;

    private GameObject target;

    protected int direction;

    protected float distance;
    protected string allyValue;
    protected string targetValue;

    protected bool isDust;
    protected bool isMove;

    protected float attackTime = 0;

    private bool IsAlive => HP > 0;

    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRen;

    protected override void OnEnable()
    {
        base.OnEnable();

        isDust = true;
        isMove = true;
    }

    private void OnDisable()
    {
        isDust = false;
        isMove = false;
    }

    public virtual void UnitSetup(UnitData _unitData)
    {
        //ChangeAc();
        unitData = _unitData;

        originHealth = _unitData.HP;
        hp = _unitData.HP;
        ap = _unitData.Ap;

        moveSpeed = _unitData.MoveSpeed;
        distance = _unitData.Distance;

        spriteRen.sprite = unitData.sprite;
        anim.runtimeAnimatorController = unitData.animCtrl;
        
        spriteRen.color = Color.white;
    }

/*    public virtual void UnitSetup(UnitData unitData)
    {
        mData = unitData;
        //ChangeAc();

        originHealth = unitData.Data.HP;
        HP = unitData.Data.HP;
        ap = unitData.Data.Ap;

        moveSpeed = unitData.Data.MoveSpeed;
        distance = unitData.Data.Distance;

        if (unitData.sprite == null)
            PopUpManager.Instance.PopUp("sprite is empty!", EPopUpType.Caution);
        else if(unitData.animCtrl == null)
            PopUpManager.Instance.PopUp("animCtrl is empty!", EPopUpType.Caution);

        mData.sprite = unitData.sprite;
        spriteRen.sprite = mData.sprite;
        spriteRen.color = Color.white;

        mData.animCtrl = unitData.animCtrl;
        anim.runtimeAnimatorController = mData.animCtrl;
    }*/

    public virtual void ChangeAc()
    {
        anim.runtimeAnimatorController =
            DataManager.Instance.HumalData.GetHumalAnimCtrl(unitData.ID);
    }

    protected void TeamValueSet()
    {
        direction = (this.gameObject.layer == 9) ? 1 : -1; //Team Value

        allyValue = (direction == 1) ? "Humal" : "Enemy"; //Layer Mask - Check Ally
        targetValue = (direction == 1) ? "Enemy" : "Humal"; //Layer Mask - Check Enemy
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
                case EUnitJob.ShortRange:
                    if(target.TryGetComponent(out LivingEntity livingEntity))
                    {
                        DamageMessage dmgMsg = new DamageMessage(
                            this.gameObject, ap, transform.position);
                        livingEntity.ApplyDamage(dmgMsg);
                    }
                    break;
                
                case EUnitJob.LongRange:
                    Shot();
                    break;
                    //case EUnitJob.Wizard: Spell(target.transform.position); break;
            }
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

    public override void Die()
    {
        anim.SetTrigger("doDie");
        StartCoroutine(DeathAnim());
    }


    private IEnumerator DeathAnim()
    {
        spriteRen.color = Color.gray;
        Color alpha = spriteRen.color;

        float time = 0f;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / 1f;
            alpha.a = Mathf.Lerp(1, 0, time);
            spriteRen.color = alpha;
            yield return null;
        }

        Dead = true;
        OnDeathAction?.Invoke();

        yield return null;
    }
}