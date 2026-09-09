using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "LevelFinishFeedbackConfig", menuName = "DuckDoku/Level Finish Feedback Config")]
    public class LevelFinishFeedbackConfig : ScriptableObject
    {
        [Header("Баннер")]
        [SerializeField] private float _victoryRevealDuration = 0.35f;
        [SerializeField] private float _defeatRevealDuration = 0.3f;

        [Header("Звёзды")]
        [SerializeField] private Sprite _earnedSprite;
        [SerializeField] private Sprite _unearnedSprite;
        [SerializeField] private float _starPopDuration = 0.3f;
        [SerializeField] private float _starDelayStep = 0.15f;

        [Header("Монеты")]
        [SerializeField] private float _coinsRevealDelayAfterStars = 0.25f;
        [SerializeField] private float _coinsPopDuration = 0.25f;
        [SerializeField] private float _coinsCountDuration = 0.6f;

        public float VictoryRevealDuration => _victoryRevealDuration;
        public float DefeatRevealDuration => _defeatRevealDuration;

        public Sprite EarnedSprite => _earnedSprite;
        public Sprite UnearnedSprite => _unearnedSprite;
        public float StarPopDuration => _starPopDuration;
        public float StarDelayStep => _starDelayStep;

        public float CoinsRevealDelayAfterStars => _coinsRevealDelayAfterStars;
        public float CoinsPopDuration => _coinsPopDuration;
        public float CoinsCountDuration => _coinsCountDuration;
    }
}
