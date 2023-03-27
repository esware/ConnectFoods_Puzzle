using System;
using System.Collections;
using DefaultNamespace;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public struct GameEvents
    {
        public static Action LoseEvent;
        public static Action WinEvent;
        public static Action GameStartEvent;



    }
    public class GameManager:MonoBehaviour
    {
        [HideInInspector] public static GameManager Instance; 
        public GameData gameData;
        public LevelData levelData;

        private const int LevelResetIndex = 1;
        private int _currentLevel;
        private int _levelIndex;
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
            InitLevelData();
        }

        private void OnEnable()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "GameScene")
            {
                _levelIndex = PlayerPrefs.GetInt("CurrentLevel");
                Instantiate(gameData.levelDatas[_levelIndex].levelObject);
            }
        }

        private void InitLevelData()
        {
            levelData = gameData.levelDatas[_levelIndex];
            
#if UNITY_EDITOR
            Debug.Log($"Current Level: {_levelIndex.ToString()}");
#endif
            
        }
        public static void IncreasePlayerPrefLevel()
        {
            PlayerPrefs.SetInt("PlayerLevel", PlayerPrefs.GetInt("PlayerLevel") + 1);
        }
        public void LoadLevel()
        {
            Transition.LoadLevel("GameScene", .5f, Color.black);
            GameEvents.GameStartEvent?.Invoke();
        }
        public void CreateLevel(GameObject levelPrefab,Transform levelTransform)
        {
            for (int i = 0; i < gameData.levelDatas.Length; i++)
            {
                var levelGameObject = Instantiate(levelPrefab,Vector3.zero,Quaternion.identity,levelTransform);
                var level = levelGameObject.GetComponent<Levels>();
                level.levelData = gameData.levelDatas[i];
                level.levelIndex = i+1;
            }
        }
    }
}