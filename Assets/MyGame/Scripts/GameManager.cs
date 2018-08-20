using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    //Food is finished, you have been starved to death.
    private static GameManager _instance;

    public static GameManager _Instance
    {
        get
        {
            return _instance;
        }
    }

    public int level = 10;
    public int startFood = 20;
    public int curFood = 20;
    [HideInInspector]
    public bool isEnd = false;
    public AudioClip deadClip;

    private Text foodText;
    private GameObject endTextGo;
    private Text dayText;
    private GameObject loadMask;
    private GameObject restartBtn;
    private GameObject inputUI;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManage Awake");
        InitGame();
    }

    private void InitGame()
    {
        restartBtn = GameObject.Find("Restart");
        Button resetBtn = restartBtn.GetComponent<Button>();
        resetBtn.onClick.AddListener(Restart);
        restartBtn.SetActive(false);

        inputUI = GameObject.Find("InputKeyUI");

        GetComponent<MapManager>().InitMap();
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        endTextGo = GameObject.Find("EndText");
        loadMask = GameObject.Find("LoadMask");
        dayText = loadMask.GetComponentInChildren<Text>();

        UpdateFoodText(0);
        endTextGo.SetActive(false);

        UpdateDayText();
    }

    private void UpdateFoodText(int foodValue)
    {
        if(foodValue == 0)
        {
            foodText.text = "Food  ：" + curFood;
        }
        else
        {
            string foodValueStr = foodValue > 0 ? "+" + foodValue : foodValue.ToString();
            foodText.text = foodValueStr + " Food  ：" + curFood; 
        }
    }

    public void IncrementFood(int value)
    {
        curFood += value;
        UpdateFoodText(value);
    }

    public void DecrementFood(int value)
    {
        curFood -= value;
        UpdateFoodText(-value);

        if (curFood <= 0)
        {
            isEnd = true;
            endTextGo.SetActive(true);
            AudioManager.Instance.RandomPlay(deadClip);
            AudioManager.Instance.StopBGM();
            restartBtn.SetActive(true);
            inputUI.SetActive(false);
        }
    }

    public void PlayerToExit()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Restart()
    {
        Debug.Log("Restart");
        Application.LoadLevel(Application.loadedLevel);
        level = 0;
        curFood = startFood;
        isEnd = false;
        inputUI.SetActive(true);
        AudioManager.Instance.PlayBGM();
    }

    private void UpdateDayText()
    {
        loadMask.SetActive(true);
        dayText.text = "Day : " + level;
        StartCoroutine(SetLoadMaskActive(false,1));
    }

    IEnumerator SetLoadMaskActive(bool isActive, int time)
    {
        yield return new WaitForSeconds(time);
        loadMask.SetActive(isActive);
    }

    void OnLevelWasLoaded(int sceneLevel)
    {
        level++;
        Debug.Log(level);
        InitGame();
    }
}
