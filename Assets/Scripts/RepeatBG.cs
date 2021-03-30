using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBG : MonoBehaviour
{
    private float speed = 0.01f;

    [SerializeField]
    private float posVal;

    Vector2 startPos;
    private float newPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        //newPos = Mathf.Repeat(Time.time * speed, posVal);
        transform.Translate(Vector2.left * speed);
        //transform.position = startPos + Vector2.left * newPos;
    }
}