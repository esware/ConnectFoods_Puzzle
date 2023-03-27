using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DefaultNamespace;

public class UIManager:MonoBehaviour
{
    [HideInInspector] public static UIManager Instance;
    [Space, Header("Game Scene Components")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI levelText;
    public Transform goalsTransform;
    public GameObject winObject;
    public GameObject loseObject;
    private int _level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        SignUpEvents();
        Time.timeScale = (0.6f);
        GridManager.Instance.goals.InitGoals(goalsTransform);
    }
    private void SignUpEvents()
    {
        
    }
    public void ShowWinPanel()
    {
        winObject.SetActive(true);
    }

    public void ShowLosePanel()
    {
        loseObject.SetActive(true);
    }

    private void Update()
    {
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (GridManager.Instance ==null)
            return;
            
        movesText.text = GridManager.Instance.leftMoves.ToString();
        levelText.text = "Level " +(PlayerPrefs.GetInt("CurrentLevel")+1).ToString() ;
    }
        
    public void NextLevel()
    {
        if (!GameManager.Instance.levelData.isPlayed)
        {
            PlayerPrefs.SetInt("PlayerLevel",PlayerPrefs.GetInt("PlayerLevel")+1);
            GameManager.Instance.levelData.isPlayed = true;
        }
        PlayerPrefs.SetInt("PlayerLevel",PlayerPrefs.GetInt("PlayerLevel")+1);
        Transition.LoadLevel("MainScene",0.5f,Color.black);
    }

}