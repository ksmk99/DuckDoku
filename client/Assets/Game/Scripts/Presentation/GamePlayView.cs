using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class GamePlayView : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;

        public event Action NextRequested;

        public void SetNextEnabled(bool enabled)
        {
            _nextButton.interactable = enabled;
        }

        private void Awake()
        {
            _nextButton.onClick.AddListener(OnNextClicked);
        }

        private void OnNextClicked()
        {
            NextRequested?.Invoke();
        }
    }
}
