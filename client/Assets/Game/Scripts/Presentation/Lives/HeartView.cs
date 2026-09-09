using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class HeartView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        private LivesFeedbackConfig _feedback;

        public void Setup(LivesFeedbackConfig feedback)
        {
            _feedback = feedback;

            ShowFull();
        }

        public void ShowFull()
        {
            _icon.transform.DOKill();

            _icon.color = _feedback.FullColor;
            _icon.transform.localScale = Vector3.one;
        }

        public void PlayLost()
        {
            _icon.transform.DOKill();

            _icon.color = _feedback.EmptyColor;
            _icon.transform.DOPunchScale(_feedback.PunchStrength, _feedback.PunchDuration, _feedback.PunchVibrato);
        }
    }
}
