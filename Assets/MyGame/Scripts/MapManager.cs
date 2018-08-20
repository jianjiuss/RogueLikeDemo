using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public static MapManager _Instance;

    public GameObject[] outWalls;
    public GameObject[] floors;
    public GameObject[] walls;
    public GameObject[] foods;
    public GameObject[] enemys;
    public GameObject exit;

    private int row = 10;
    private int col = 10;

    private int maxWallCount = 8;
    private int minWallCount = 2;

    private Transform mapContainer;
    private List<Vector3> positionList;

    private GameManager gameManager;
    public List<Vector2> units = new List<Vector2>();

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        _Instance = this;
    }
	
	void Update () 
    {
		
	}

    public void InitMap()
    {
        units.Clear();
        mapContainer = new GameObject("Map").transform;
        //背景地图生成
        for(int y = 0; y < row; y++)
        {
            for(int x = 0; x < col; x++)
            {
                if(x == 0 || y == 0 || x == (col - 1) || y == (row - 1))
                {
                    GameObject outWall = RandomPrefab(outWalls);
                    GameObject.Instantiate(outWall, new Vector3(x, y, 0), Quaternion.identity).transform.parent = mapContainer;
                }
                else
                {
                    GameObject floor = RandomPrefab(floors);
                    GameObject.Instantiate(floor, new Vector3(x, y, 0), Quaternion.identity).transform.parent = mapContainer;
                }
            }
        }

        //存储物品随机生成位置
        positionList = new List<Vector3>();
        for(int y = 2; y < row - 2; y++)
        {
            for(int x = 2; x < col - 2; x++)
            {
                positionList.Add(new Vector2(x, y));
            }
        }

        //障碍物生成
        int wallCount = Random.Range(minWallCount, maxWallCount);
        CreateItems(wallCount, walls);

        //食物生成
        //食物数量 2 - level * 2
        int foodMax = gameManager.level * 2 + 1;
        if(foodMax > 8)
        {
            foodMax = 8;
        }
        int foodCount = Random.Range(2, foodMax);
        CreateItems(foodCount, foods);

        //敌人生成
        int enemyCount = gameManager.level / 2;
        if(enemyCount > 12)
        {
            enemyCount = 12;
        }
        CreateItems(enemyCount, enemys);

        //生成出口
        GameObject.Instantiate(exit, new Vector2(col - 2, row - 2), Quaternion.identity).transform.parent = mapContainer;
    }

    private void CreateItems(int count, GameObject[] prefabs)
    {
        if(positionList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = RandomPosition();
            GameObject go = RandomPrefab(prefabs);
            GameObject.Instantiate(go, pos, Quaternion.identity).transform.parent = mapContainer;
        }
    }

    private Vector2 RandomPosition()
    {
        int positionIndex = 0;
        try
        {
            positionIndex = Random.Range(0, positionList.Count);
            Vector2 pos = positionList[positionIndex];
            positionList.RemoveAt(positionIndex);

            return pos;
        }
        catch
        {
            Debug.Log(positionList.Count);
            Debug.Log(positionIndex);
            throw;
        }
    }

    private GameObject RandomPrefab(GameObject[] prefabs)
    {
        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }
}
