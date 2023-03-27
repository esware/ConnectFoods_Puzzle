using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPanelController : MonoBehaviour
{
    [SerializeField] private Image[] revealImages;
    [SerializeField] private Sprite star;

    private float _currentPercentage;
    private float _targetPercentage;
    
    private void Start()
    {
        CreateStar();
    }


    private void CreateStar()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.levelData.isPlayed = true;
        }
        for (int i = 0; i < GameManager.Instance.levelData.CalculateStars(); i++)
        {
            revealImages[i].sprite = star;
        }
    }
}