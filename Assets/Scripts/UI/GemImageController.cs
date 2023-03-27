using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GemImageController : MonoBehaviour
    {
        [HideInInspector] public Action<GemImageController> GemImageArrivedEvent;

        [SerializeField] private float shrinkSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float fadeOutSpeed;

        private RectTransform _rectTransform;
        private Image _image;

        private Color _imageColor;

        private Vector3 _targetScale;
        private Vector2 _targetPos;

        private Vector3 _defScale;
        private void Awake()
        {
            InitComponents();

            _imageColor = _image.color;
            _defScale = _rectTransform.localScale;
        }

        private void InitComponents()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        public void Activate(Vector2 anchorPos, Vector2 targetPos)
        {
            gameObject.SetActive(true);
            
            _targetPos = targetPos;
            _rectTransform.anchoredPosition = anchorPos;

            SetToDefaults();
            
            StartCoroutine(MovementLoop());
        }

        private IEnumerator MovementLoop()
        {
            while (Vector2.Distance(_rectTransform.localPosition, _targetPos) > 0.1f)
            {
                Movement();
                FadeOut();
                Shrink();
                
                yield return null;
            }

            Deactivate();
        }

        public void Deactivate()
        {
            GemImageArrivedEvent?.Invoke(this);
            gameObject.SetActive(false);
        }
        
        private void Movement()
        {
            _rectTransform.localPosition =
                Vector2.MoveTowards(_rectTransform.localPosition, _targetPos, Time.deltaTime * movementSpeed);
        }

        private void SetToDefaults()
        {
            _imageColor.a = 1f;
            _image.color = _imageColor;

            _rectTransform.localScale = _defScale;
        }
        
        private void Shrink()
        {
            _rectTransform.localScale -= Time.deltaTime * shrinkSpeed * Vector3.one;
        }
        
        private void FadeOut()
        {
            _imageColor.a -= Time.deltaTime * fadeOutSpeed;
            _image.color = _imageColor; 
        }

        private void OnDestroy()
        {
            GemImageArrivedEvent = null;
        }
    }
}
