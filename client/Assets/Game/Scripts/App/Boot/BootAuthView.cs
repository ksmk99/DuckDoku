using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.App
{
    public class BootAuthView : MonoBehaviour
    {
        private const float SpinDurationSeconds = 1f;

        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private RectTransform _spinnerIcon;

        public event Action RetryRequested;

        public void ShowStatus(string text)
        {
            _statusText.text = text;
            _retryButton.gameObject.SetActive(false);
            _spinnerIcon.gameObject.SetActive(true);
        }

        public void ShowError(string text)
        {
            _statusText.text = text;
            _retryButton.gameObject.SetActive(true);
            _spinnerIcon.gameObject.SetActive(false);
        }

        private void Awake()
        {
            _retryButton.gameObject.SetActive(false);
            _retryButton.onClick.AddListener(OnRetryClicked);

            _spinnerIcon.DORotate(new Vector3(0f, 0f, -360f), SpinDurationSeconds, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDestroy()
        {
            _spinnerIcon.DOKill();
        }

        private void OnRetryClicked()
        {
            RetryRequested?.Invoke();
        }
    }
}
