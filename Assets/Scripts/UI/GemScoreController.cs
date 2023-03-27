using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UI;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GemScoreController : MonoBehaviour
{
    [Tooltip("Gem Image Prefab to instantiate")]
    [SerializeField] private GemImageController gemImage;
    
    [HideInInspector] public int gemCount;
    
    private TextMeshProUGUI _scoreText;
    private RectTransform _parentRectTransform;
    
    private HashSet<GemImageController> _gemImagePoolSet = new HashSet<GemImageController>();

    private void Awake()
    {
        InitComponents();
    }

    private void Start()
    {
        SignUpEvents();
        
        CreateGemImagePool();
        ChangeScoreText();
    }

    private void InitComponents()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
        if (!_scoreText)
        {
            Debug.LogError($"Doesn't have TextMeshProUGUI component");
        }
#endif
        _parentRectTransform = transform.parent.transform.GetComponent<RectTransform>();
    }

    private void SignUpEvents()
    {
        //GameEvents.CollectableEvent += ActivateGemImage;
    }
    
    private void CreateGemImagePool()
    {
        for (int i = 0; i < 10; i++)
        {
            var gemImageController = Instantiate(gemImage.gameObject, new Vector3(1000f, 1000f, 1000f), Quaternion.identity, _parentRectTransform.transform)
                .GetComponent<GemImageController>();

            gemImageController.GemImageArrivedEvent += OnGemImageArrived;
            gemImageController.gameObject.SetActive(false);
                
            _gemImagePoolSet.Add(gemImageController);
        }
    }
    
    private void ActivateGemImage(Vector3 gemPosition)
    {
        var gemScreenPos = Camera.main.WorldToScreenPoint(gemPosition);
        Vector3 gemImageCanvasPos;
            
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_parentRectTransform, gemScreenPos, Camera.main, out gemImageCanvasPos);
            
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, gemScreenPos, guiCamera, out gemImageCanvasPos);

        var gemImageController = GetGemImageFromPool();
            
        gemImageController.Activate(gemImageCanvasPos, _scoreText.rectTransform.localPosition);
        
        gemCount++;
    }
    
    private GemImageController GetGemImageFromPool()
    {
        if (_gemImagePoolSet.Count < 1)
        {
            CreateGemImagePool();
        }

        return _gemImagePoolSet.First();
    }
    
    private void OnGemImageArrived(GemImageController gemImageController)
    {
        _gemImagePoolSet.Add(gemImageController);
        ChangeScoreText();
    }

    private void ChangeScoreText()
    {
        _scoreText.text = gemCount.ToString();
    }
}
