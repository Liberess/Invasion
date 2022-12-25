using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroShadow : MonoBehaviour
{
    public Hero hero;

    private void Start()
    {
        float y = 0;

        switch(hero.MyStat.ID)
        {
            default: y = -0.05f; break;
            case 0: y = -0.05f; break;
            case 3: y = -0.14f; break;
            case 6: y = -0.12f; break;
            case 10: y = -0.16f; break;
            case 11: y = -0.16f; break;
        }

        transform.position = new Vector3(hero.transform.position.x,
            hero.transform.position.y + y, hero.transform.position.z);
    }
}