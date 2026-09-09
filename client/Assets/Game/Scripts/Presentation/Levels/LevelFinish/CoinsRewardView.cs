using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class CoinsRewardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;

        private LevelFinishFeedbackConfig _feedback;
        private Sequence _sequence;
        private int _displayedAmount;

        public void Setup(LevelFinishFeedbackConfig feedback)
        {
            _feedback = feedback;

            Hide();
        }

        public void Hide()
        {
            _sequence?.Kill();

            transform.localScale = Vector3.zero;
            _displayedAmount = 0;
            _amountText.text = "0";
        }

        public void PlayReward(int amount, float delay)
        {
            Hide();

            _sequence = DOTween.Sequence();
            _sequence.SetDelay(delay);
            _sequence.Append(transform.DOScale(Vector3.one, _feedback.CoinsPopDuration).SetEase(Ease.OutBack));
            _sequence.Append(DOTween.To(() => _displayedAmount, SetDisplayedAmount, amount, _feedback.CoinsCountDuration).SetEase(Ease.OutQuad));
        }

        private void SetDisplayedAmount(int value)
        {
            _displayedAmount = value;
            _amountText.text = value.ToString();
        }
    }
}
