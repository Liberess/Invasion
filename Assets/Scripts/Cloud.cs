using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float speed = 0.1f;

    private void FixedUpdate()
    {
        float x = speed * Time.deltaTime;

        transform.Translate(new Vector3(x, 0));

        if (transform.position.x >= 8)
        {
            transform.position = new Vector3(-9, transform.position.y);
        }
    }
}