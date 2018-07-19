using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject[] outWalls;
    public GameObject[] floors;
    public GameObject[] walls;

    private int row = 10;
    private int col = 10;

    private int maxWallCount = 8;
    private int minWallCount = 2;

    private Transform mapContainer;
    private List<Vector3> positionList;
	void Start () 
    {
        InitMap();
	}
	
	void Update () 
    {
		
	}

    private void InitMap()
    {
        mapContainer = new GameObject("Map").transform;
        for(int y = 0; y < row; y++)
        {
            for(int x = 0; x < col; x++)
            {
                if(x == 0 || y == 0 || x == (col - 1) || y == (row - 1))
                {
                    int value = Random.Range(0, outWalls.Length);
                    GameObject.Instantiate(outWalls[value], new Vector3(x, y, 0), Quaternion.identity).transform.parent = mapContainer;
                }
                else
                {
                    int value = Random.Range(0, floors.Length);
                    GameObject.Instantiate(floors[value], new Vector3(x, y, 0), Quaternion.identity).transform.parent = mapContainer;
                }
            }
        }

        positionList = new List<Vector3>();
        for(int y = 2; y < row - 2; y++)
        {
            for(int x = 2; x < col - 2; x++)
            {
                positionList.Add(new Vector2(x, y));
            }
        }

        int wallCount = Random.Range(minWallCount, maxWallCount);
        for(int i = 0; i < wallCount; i++)
        {
            int positionIndex = Random.Range(0, positionList.Count);
            Vector2 pos = positionList[positionIndex];
            positionList.RemoveAt(positionIndex);

            int wallIndex = Random.Range(0, walls.Length);
            GameObject.Instantiate(walls[wallIndex], pos, Quaternion.identity).transform.parent = mapContainer;
        }
    }
}
