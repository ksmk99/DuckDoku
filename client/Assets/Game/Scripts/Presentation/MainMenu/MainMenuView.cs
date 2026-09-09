using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private MainMenuFeedbackConfig _feedback;

        public event Action PlayClicked;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(Click);
        }

        public void PlayDenied()
        {
            _playButton.transform.DOKill();
            _playButton.transform.DOShakePosition(_feedback.ShakeDuration, _feedback.ShakeStrength, _feedback.ShakeVibrato);
        }

        private void Click()
        {
            PlayClicked?.Invoke();
        }
    }
}
