using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHero : Unit
{
    [SerializeField] private HumalData humalData;
    public HumalData HumalData => humalData;

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

    public override void UnitSetup(UnitData unitData)
    {
        base.UnitSetup(unitData);
    }

    public void UnitSetup(HumalData humalData)
    {
        this.humalData = humalData;
    }

    #region 이동 세팅
    private void Movement()
    {
        if (isMove)
        {
            //anim.SetBool("isWalk", true);
            rigid.velocity = new Vector3(speedX, speedY, speedY);
        }
        else
        {
            //anim.SetBool("isWalk", false);
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
            speedX *= -0.5f;
            speedY *= -0.5f;

            FlipX();
        }
    }
}