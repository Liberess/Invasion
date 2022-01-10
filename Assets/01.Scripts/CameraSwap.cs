using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwap : MonoBehaviour
{
    [SerializeField, Range(0f, 5f)] private float speed = 0.1f;
    [SerializeField] private Transform[] canvasPosArray;

    public void SetMove(DircType type)
    {
        if (canvasPosArray[(int)type.Type] == null)
            return;

        StartCoroutine(MoveCo(canvasPosArray[(int)type.Type].position));
    }

    private IEnumerator MoveCo(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);

        while(distance > 0f)
        {
            distance = Vector3.Distance(transform.position, target);
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);

/*            Vector3 velocity = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, target, speed);*/

            yield return new WaitForSeconds(0.01f);
        }
    }
}