using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CanvasController : MonoBehaviour
    {
        [Header("Main Scene Components")]
        public GameObject mainCanvas;
        public GameObject levelPopup;
        public GameObject levelPrefab;
        public Transform levelTransform;

        private void Start()
        {
            GameManager.Instance.CreateLevel(levelPrefab, levelTransform);

            if (PlayerPrefs.GetInt("PlayerLevel") >= 2)
            {
                mainCanvas.SetActive(false);
                levelPopup.SetActive(true);
            }
        }
        public void GetLevelPopup()
        {
            StartCoroutine(GetLevelPopupC());
        }

        public void GetMainPopup()
        {
            mainCanvas.SetActive(true);
            levelPopup.SetActive(false);
        }

        private IEnumerator GetLevelPopupC()
        {
            yield return new WaitForSeconds(.5f);
            mainCanvas.SetActive(false);
            levelPopup.SetActive(true);
        }
    }
}