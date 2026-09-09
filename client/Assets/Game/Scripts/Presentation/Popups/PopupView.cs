using System;
using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public abstract class PopupView : MonoBehaviour, IPopupHandle
    {
        [SerializeField] private Button _backdropButton;
        [SerializeField] private Button _closeButton;

        public event Action Closed;

        protected virtual void Awake()
        {
            if (_backdropButton != null)
            {
                _backdropButton.onClick.AddListener(RaiseClosed);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(RaiseClosed);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Release()
        {
            GameObject.Destroy(gameObject);
        }

        private void RaiseClosed()
        {
            Closed?.Invoke();
        }
    }
}
