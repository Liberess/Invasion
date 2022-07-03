using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float offset;

    private void LateUpdate()
    {
        if (CameraMove.Instance.isMove) //BackGround Movement
            transform.Translate(new Vector3(CameraMove.Instance.speed * offset * Time.deltaTime, 0, 0));
    }
}