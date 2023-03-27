using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Levels : MonoBehaviour
{
    
    [HideInInspector] public int levelIndex;
    [HideInInspector] public LevelData levelData;
    
    [SerializeField] private Transform levelStarsTransform;
    [SerializeField] private GameObject levelLocked;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Sprite starSprite;
    

    private void OnEnable()
    {
        InitData();
    }

    void InitData()
    {
        levelText.text = "Level " + levelIndex;
        
        if (PlayerPrefs.GetInt("PlayerLevel")+1 >= levelIndex)
        {
            this.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
            levelStarsTransform.gameObject.SetActive(true);
            levelLocked.gameObject.SetActive(false);

            if (levelData.isPlayed)
            {
                LoadData();
            }
        }
        else
        {
            this.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
            levelStarsTransform.gameObject.SetActive(false);
            levelLocked.gameObject.SetActive(true);
        }

    }

    private void LoadData()
    {
        var g = new GameObject();
        for (int i = 0; i < levelData.CalculateStars(); i++)
        {
            var star = Instantiate(g, Vector3.zero, Quaternion.identity,levelStarsTransform);
            star.AddComponent<Image>().sprite = starSprite;
        }
    }


    public void PlayLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel",levelIndex-1);
        GameManager.Instance.LoadLevel();
    }
}