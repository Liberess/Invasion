using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldHero : MonoBehaviour
{
    #region 변수 선언
    private DataManager dataManager;

    public GameObject shadow;

    public RuntimeAnimatorController[] LevelAc;

    public int mID;
    public float mExp;
    public int mLevel;

    private bool isPick;
    private float pickTime = 0;
    private float maxPickTime = 0.5f;

    public bool isMove;
    private bool isBorder;

    public float speedX;
    public float speedY;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    #endregion

    private void Awake()
    {
        isMove = true;
        isPick = false;
        isBorder = false;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        Think();
    }

    private void Start()
    {
        dataManager = DataManager.Instance;
        StartCoroutine(SetSoulGem());
    }

    private void Update()
    {
        SetExp();
        MonsterPick();
    }

    private void LateUpdate()
    {
        dataManager.heroData.heroDic[name].ID = mID;
        dataManager.heroData.heroDic[name].exp = mExp;
        dataManager.heroData.heroDic[name].level = mLevel;
    }

    private void FixedUpdate()
    {
        #region 이동 실행
        //Move
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
        #endregion
    }

    #region 이동 세팅
    private void Think()
    {
        speedX = UnityEngine.Random.Range(-0.5f, 0.5f);
        speedY = UnityEngine.Random.Range(-0.5f, 0.5f);

        FlipX();

        int delay = UnityEngine.Random.Range(2, 5);

        Invoke("Think", delay);
    }

    private void FlipX()
    {
        if (isMove)
        {
            if (speedX > 0)
            {
                sprite.flipX = false;
            }
            else if (speedX < 0)
            {
                sprite.flipX = true;
            }
        }
    }
    #endregion

    #region 재화 습득(Karma, Gold)
    IEnumerator SetSoulGem()
    {
        DataManager.Instance.gameData.soulGem += mID + 1 * mLevel;

        yield return new WaitForSeconds(3f);

        StartCoroutine(SetSoulGem());
    }

    public void GetSoulGem()
    {
        //DataManager.Instance.gameData.soulGem += mID + 1 * mLevel * DataManager.Instance.gameData.clickLevel;
    }
    #endregion

    #region 성장
    public void ChangeAc()
    {
        if (mLevel != 0)
        {
            anim.runtimeAnimatorController = LevelAc[mLevel - 1];
        }
    }

    private void SetExp()
    {
        if (mLevel != 3)
        {
            if (mExp < (mLevel * 50))
            {
                mExp += Time.deltaTime;
            }
            else
            {
                mLevel += 1;
                SoundManager.Instance.PlaySFX("Grow");
                ChangeAc();
            }
        }
    }
    #endregion

    #region 몬스터 선택
    private void MonsterPick()
    {
        if (isPick)
        {
            if (pickTime >= maxPickTime)
            {
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 objPos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = objPos;
            }
            else
            {
                pickTime += Time.deltaTime;
            }
        }
        else
        {
            pickTime = 0;
        }
    }
    #endregion

    #region 물리 충돌
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
    #endregion

    #region 마우스 클릭 이벤트
    private void OnMouseDown()
    {
        if (GameManager.Instance.isPlay)
        {
            sprite.sortingOrder += 1;

            isMove = false;

            if (mExp < 150)
            {
                mExp += 1;
            }

            GetSoulGem();

            anim.SetTrigger("doTouch");

            SoundManager.Instance.PlaySFX("Touch");
        }
    }

    private void OnMouseDrag()
    {
        if (GameManager.Instance.isPlay)
        {
            isPick = true;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isPlay)
        {
            sprite.sortingOrder -= 1;

            isPick = false;
            isMove = true;

            if (isBorder)
            {
                transform.position = new Vector3(0, 0, 0);
            }
        }
    }
    #endregion
}