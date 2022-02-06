using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public static CameraMove Instance { get; private set; }

    private bool canMove => transform.position.x > -8.5 && direction == -1
        || transform.position.x < 8.5 && direction == 1;

    public int speed { get; private set; }
    public bool isMove { get; private set; }

    private int direction = 0;
    private float accel = 1f;

    private Vector3 prePos, movePos, originPos, targetPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        speed = 5;
        isMove = false;
        originPos = transform.position;
    }

    private void Update()
    {
        InputMouse();

        if(isMove && canMove)
            transform.Translate(Vector3.right * direction * speed * accel * Time.deltaTime);
    }

    private void InputMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            StopCoroutine(DecelerateCoru());

            isMove = true;
            accel = 1f;
            direction = 0;

            prePos = Input.mousePosition;
            prePos = Camera.main.ScreenToWorldPoint(prePos);
            prePos.z = originPos.z;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            StartCoroutine(DecelerateCoru());
        }
        else if (Input.GetMouseButton(2))
        {
            isMove = true;

            movePos = Input.mousePosition;
            movePos = Camera.main.ScreenToWorldPoint(movePos);
            movePos.z = originPos.z;

            targetPos = prePos - movePos;

            float distance = Vector3.Distance(prePos, movePos);

            // Set Move Direction
            if (targetPos.x > 0) // 오른쪽으로 가는 중
                direction = 1;
            else
                direction = -1;

            if (accel < 3f)
                accel = distance;
        }
    }

    private IEnumerator DecelerateCoru()
    {
        while(accel > 0f)
        {
            accel -= speed * 1.5f * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }

        isMove = false;
    }
}