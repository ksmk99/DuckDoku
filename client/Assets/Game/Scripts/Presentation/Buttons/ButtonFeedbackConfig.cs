using DG.Tweening;
using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "ButtonFeedbackConfig", menuName = "DuckDoku/Button Feedback Config")]
    public class ButtonFeedbackConfig : ScriptableObject
    {
        [Header("Press")]
        [SerializeField] private float _pressScale = 0.94f;
        [SerializeField] private float _pressDuration = 0.07f;
        [SerializeField] private Ease _pressEase = Ease.OutQuad;
        [SerializeField] private float _releaseDuration = 0.11f;
        [SerializeField] private Ease _releaseEase = Ease.OutQuad;

        [Header("Hover")]
        [SerializeField] private float _hoverScale = 1.05f;
        [SerializeField] private float _hoverDuration = 0.12f;
        [SerializeField] private Ease _hoverEase = Ease.OutQuad;

        public float PressScale => _pressScale;
        public float PressDuration => _pressDuration;
        public Ease PressEase => _pressEase;
        public float ReleaseDuration => _releaseDuration;
        public Ease ReleaseEase => _releaseEase;

        public float HoverScale => _hoverScale;
        public float HoverDuration => _hoverDuration;
        public Ease HoverEase => _hoverEase;
    }
}
