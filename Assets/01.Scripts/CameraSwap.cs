using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwap : MonoBehaviour
{
    private bool canSwap = true;
    [SerializeField, Range(0f, 5f)] private float speed = 0.1f;
    [SerializeField] private Transform[] canvasPosArray;

    public void SetMove(DircType type)
    {
        if (!canSwap)
            return;

        if (canvasPosArray[(int)type.Type] == null)
            return;

        var pos = canvasPosArray[(int)type.Type].position;
        pos.z = -10f;

        canSwap = false;

        StartCoroutine(MoveCo(pos));
    }

    private IEnumerator MoveCo(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);

        while(distance > 0f)
        {
            distance = Vector3.Distance(transform.position, target);
            transform.position = Vector3.Lerp(transform.position, target, speed);

            if (distance <= 0.001f)
                break;

            yield return new WaitForSeconds(0.01f);
        }

        canSwap = true;
    }
}