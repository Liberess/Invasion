using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHero : Unit
{
    private bool isBorder = false;

    [SerializeField] private float speedX;
    [SerializeField] private float speedY;

    private void Awake()
    {
        isDust = false;
        isMove = true;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Think();
    }

    private void Update()
    {
        Movement();
    }

    protected override void CustomUnitSetup(UnitStatus status)
    {
        myStat = status;
        myStat.mySprite = DataManager.Instance.heroData.heroSpriteList[status.ID];
        sprite.sprite = myStat.mySprite;
        myStat.animCtrl = DataManager.Instance.heroData.heroAnimCtrlList[status.ID];
        anim.runtimeAnimatorController = myStat.animCtrl;
    }

    #region 이동 세팅
    private void Movement()
    {
        if (isMove)
        {
            anim.SetBool("isWalk", true);
            rigid.velocity = new Vector3(speedX, speedY, speedY);
        }
        else
        {
            anim.SetBool("isWalk", false);
            rigid.velocity = Vector3.zero;
        }
    }

    private void Think()
    {
        speedX = Random.Range(-0.5f, 0.5f);
        speedY = Random.Range(-0.5f, 0.5f);

        FlipX();

        int delay = Random.Range(2, 5);

        Invoke("Think", delay);
    }

    private void FlipX()
    {
        if (isMove)
        {
            if (speedX > 0)
                sprite.flipX = false;
            else if (speedX < 0)
                sprite.flipX = true;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BorderLine"))
        {
            isBorder = true;

            speedX *= -0.5f;
            speedY *= -0.5f;

            FlipX();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BorderLine"))
        {
            isBorder = false;
        }
    }
}