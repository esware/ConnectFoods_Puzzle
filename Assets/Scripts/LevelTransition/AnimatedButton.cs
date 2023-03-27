using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class AnimatedButton:UIBehaviour, IPointerDownHandler
    {
        [Serializable] 
        public class ButtonClickedEvent : UnityEvent{ }
        public bool interactable = true;
        
        [SerializeField] 
        private ButtonClickedEvent _onClick = new ButtonClickedEvent();
        private Animator _animator;

        override protected void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
        }

        public ButtonClickedEvent onClick
        {
            get => _onClick;
            set => _onClick = value;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !interactable)
                return;
            Press();

        }

        private void Press()
        {
            if (!IsActive())
                return;
            _animator.SetTrigger("Pressed");
            Invoke(nameof(InvokeOnClickAction),0.1f);
        }

        private void InvokeOnClickAction()
        {
            _onClick.Invoke();
        }
    }
}