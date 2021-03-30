using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterStatus
{
    public string name;
    public int ID;
    public float Exp;
    public int Level;

    public MonsterStatus(string _name,int _id, float _exp, int _level)
    {
        this.name = _name;
        this.ID = _id;
        this.Exp = _exp;
        this.Level = _level;
    }
}

public class Monster : MonoBehaviour
{
    #region 변수 선언
    public static Monster _instance;
    DataManager dataManager;

    public GameObject shadow;

    public RuntimeAnimatorController[] LevelAc;

    public int mID;
    public float mExp;
    public int mLevel;

    private bool isPick;
    private float pickTime = 0;
    private float maxPickTime = 0.5f;

    public bool isMove;
    private bool isSell;
    private bool isBorder;
    private bool doMerge;
    private bool canMerge;

    public float speedX;
    public float speedY;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    #endregion

    private void Awake()
    {
        #region 기본 초기화
        isMove = true;
        isPick = false;
        isSell = false;
        isBorder = false;
        canMerge = false;

        _instance = this;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        #endregion

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
        DataManager.Instance.monsData.monsDic[name].ID = mID;
        DataManager.Instance.monsData.monsDic[name].Exp = mExp;
        DataManager.Instance.monsData.monsDic[name].Level = mLevel;
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
        if(isMove)
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
        if (DataManager.Instance.gameData.soulGem < DataManager.Instance.gameData.maxSoul)
        {
            DataManager.Instance.gameData.soulGem += mID + 1 * mLevel * DataManager.Instance.gameData.clickLevel;
        }
    }
    #endregion

    #region 성장
    public void ChangeAc()
    {
        if(mLevel != 0)
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

    private void Merge(GameObject _monsObj)
    {
        if (doMerge)
        {
            GameObject mons = Instantiate(Resources.Load<GameObject>("Prefabs/Monster"), new Vector3(0, 0, 0), Quaternion.identity);
            mons.name = string.Concat("Jelly" + dataManager.monsData.monsIndex);
            mons.GetComponent<Monster>().mID = mID + 1;
            mons.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.monsSpriteList[mID + 1];

            dataManager.monsData.monsDic.Add(mons.name, new MonsterStatus(mons.name, mID + 1, 0, 1));
            dataManager.monsData.monsList = new List<MonsterStatus>(dataManager.monsData.monsDic.Values);
            dataManager.monsData.monsIndex++;

            //DataManager.Instance.monsData.monsDic.Remove(collision.gameObject.name);
            DataManager.Instance.monsData.monsDic.Remove(_monsObj.name);
            DataManager.Instance.monsData.monsDic.Remove(name);
            dataManager.ResettingMonsList();

            //Destroy(collision.gameObject);
            Destroy(_monsObj);
            Destroy(this.gameObject);
        }
    }

    #region 물리 충돌
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SellBtn"))
        {
            isSell = true;
        }

        if (collision.CompareTag("BorderLine"))
        {
            isBorder = true;

            speedX *= -0.5f;
            speedY *= -0.5f;

            FlipX();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision.GetComponent<Monster>().mID == mID)
            {
                canMerge = true;

                Merge(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SellBtn"))
        {
            isSell = false;
        }

        if (collision.CompareTag("BorderLine"))
        {
            isBorder = false;
        }

        if (collision.CompareTag("Monster"))
        {
            canMerge = false;
        }
    }
    #endregion

    #region 마우스 클릭 이벤트
    private void OnMouseDown()
    {
        if (GameManager._instance.isPlay)
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
        if (GameManager._instance.isPlay)
        {
            isPick = true;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager._instance.isPlay)
        {
            sprite.sortingOrder -= 1;

            isPick = false;
            isMove = true;
            
            if(isBorder)
            {
                transform.position = new Vector3(0, 0, 0);
            }

            if (isSell)
            {
                int gold = DataManager.Instance.gameData.goldUnit[(int)Unit.Max - 1];

                if(gold < 999)
                {
                    DataManager.Instance.gameData.gold += GameManager._instance.monsGoldList[mID] * mLevel;

                    SoundManager.Instance.PlaySFX("Sell");
                    DataManager.Instance.monsData.monsDic.Remove(name);
                    dataManager.ResettingMonsList();

                    Destroy(this.gameObject);
                }
                else
                {
                    SoundManager.Instance.PlaySFX("Fail");
                }
            }

            if(canMerge)
            {
                doMerge = true;
            }
        }
    }
    #endregion
}