using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour 
{
    public GameManager gameManagePrefab;

    void Awake()
    {
        if(GameManager._Instance == null)
        {
            GameObject.Instantiate(gameManagePrefab);
        }
    }
}
