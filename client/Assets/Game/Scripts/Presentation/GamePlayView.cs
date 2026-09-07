using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class GamePlayView : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text _livesText;

        public event Action NextRequested;

        public void SetNextEnabled(bool enabled)
        {
            _nextButton.interactable = enabled;
        }

        public void SetLivesRemaining(int remaining)
        {
            _livesText.text = remaining.ToString();
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
