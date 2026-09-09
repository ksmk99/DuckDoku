using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class LevelFinishView : MonoBehaviour
    {
        [SerializeField] private GameObject _victoryBanner;
        [SerializeField] private GameObject _defeatBanner;
        [SerializeField] private StarView[] _stars;
        [SerializeField] private LevelFinishFeedbackConfig _feedback;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _nextButton;

        public event Action RetryRequested;
        public event Action NextRequested;

        public void ShowVictory(int stars)
        {
            _victoryBanner.SetActive(true);
            _defeatBanner.SetActive(false);

            _victoryBanner.transform.DOKill();
            _victoryBanner.transform.localScale = Vector3.zero;
            _victoryBanner.transform.DOScale(Vector3.one, _feedback.VictoryRevealDuration).SetEase(Ease.OutBack);

            for (int i = 0; i < _stars.Length; i++)
            {
                if (i < stars)
                {
                    _stars[i].PlayEarned(i * _feedback.StarDelayStep);
                }
                else
                {
                    _stars[i].ShowUnearned();
                }
            }
        }

        public void ShowDefeat()
        {
            _victoryBanner.SetActive(false);
            _defeatBanner.SetActive(true);

            _defeatBanner.transform.DOKill();
            _defeatBanner.transform.localScale = Vector3.zero;
            _defeatBanner.transform.DOScale(Vector3.one, _feedback.DefeatRevealDuration).SetEase(Ease.OutQuad);
        }

        private void Awake()
        {
            foreach (StarView star in _stars)
            {
                star.Setup(_feedback);
            }

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
