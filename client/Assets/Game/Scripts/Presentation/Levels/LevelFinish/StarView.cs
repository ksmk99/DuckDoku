using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class StarView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        private LevelFinishFeedbackConfig _feedback;

        public void Setup(LevelFinishFeedbackConfig feedback)
        {
            _feedback = feedback;

            ShowUnearned();
        }

        public void ShowUnearned()
        {
            _icon.transform.DOKill();

            _icon.sprite = _feedback.UnearnedSprite;
            _icon.transform.localScale = Vector3.one;
        }

        public void PlayEarned(float delay)
        {
            _icon.transform.DOKill();

            _icon.sprite = _feedback.EarnedSprite;
            _icon.transform.localScale = Vector3.zero;

            _icon.transform.DOScale(Vector3.one, _feedback.StarPopDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);
        }
    }
}
