using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShadow : MonoBehaviour
{
    public Monster monster;

    private void Start()
    {
        float y = 0;

        switch(monster.mID)
        {
            default:
                y = -0.05f;
                break;
            case 0:
                y = -0.05f;
                break;
            case 3:
                y = -0.14f;
                break;
            case 6:
                y = -0.12f;
                break;
            case 10:
                y = -0.16f;
                break;
            case 11:
                y = -0.16f;
                break;
        }

        transform.position = new Vector3(monster.transform.position.x, monster.transform.position.y + y, monster.transform.position.z);
    }
}