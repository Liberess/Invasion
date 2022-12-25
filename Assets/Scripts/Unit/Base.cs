using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : Unit
{
    private BattleAI battleAI;
    private Text hpTxt;

    private enum BaseType { Red, Blue }

    [SerializeField] private BaseType baseType;

    private bool isRush = false;

    private Vector3 rayPos;

    private void Awake()
    {
        isDust = true;
        isMove = true;
        isRush = false;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hpTxt = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        battleMgr = BattleManager.Instance;
        battleAI = FindObjectOfType<BattleAI>();

        rayPos = new Vector3(transform.position.x,
            transform.position.y + sprite.size.y / 3f, 0f);

        hp = mMyData.hp;

        hpTxt.text = hp + "/" + mMyData.hp;
    }

    private void FixedUpdate()
    {
        if (baseType == BaseType.Blue)
            return;

        RaycastHit2D rayHit = Physics2D.Raycast(rayPos, Vector2.left, 1.5f, LayerMask.GetMask("Hero"));
        if (rayHit.collider != null && rayHit.collider.GetComponent<Hero>() != null)
        {
            if (!isRush)
            {
                isRush = true;
                battleAI.SpawnRush(isRush);
                Debug.Log("히어로가 가까이 옴! 소환 러시 시작!");
            }
        }
/*        else
        {
            if(isRush)
            {
                isRush = false;
                battleAI.SpawnRush(isRush);
                Debug.Log("히어로가 주변에 없음! 소환 러시 종료!");
            }
        }*/
    }

    protected override void Hit(int _atk)
    {
        base.Hit(_atk);

        hpTxt.text = hp + "/" + mMyData.hp;
    }

    protected override void Die()
    {
        battleMgr.GameOverAction();
        gameObject.SetActive(false);
    }
}