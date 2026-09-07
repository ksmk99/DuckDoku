using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class LevelFinishView : MonoBehaviour
    {
        [SerializeField] private GameObject _victoryBanner;
        [SerializeField] private GameObject _defeatBanner;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _nextButton;

        public event Action RetryRequested;
        public event Action NextRequested;

        public void ShowVictory()
        {
            _victoryBanner.SetActive(true);
            _defeatBanner.SetActive(false);
        }

        public void ShowDefeat()
        {
            _victoryBanner.SetActive(false);
            _defeatBanner.SetActive(true);
        }

        private void Awake()
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
            _nextButton.onClick.AddListener(OnNextClicked);
        }

        private void OnRetryClicked()
        {
            RetryRequested?.Invoke();
        }

        private void OnNextClicked()
        {
            NextRequested?.Invoke();
        }
    }
}
