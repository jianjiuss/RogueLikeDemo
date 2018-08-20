using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour 
{
    public Sprite sprite;
    private int hp = 2;

    public void TakeDamage()
    {
        hp -= 1;
        GetComponent<SpriteRenderer>().sprite = sprite;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
