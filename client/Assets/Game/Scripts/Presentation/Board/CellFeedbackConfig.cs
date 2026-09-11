using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "CellFeedbackConfig", menuName = "DuckDoku/Cell Feedback Config")]
    public class CellFeedbackConfig : ScriptableObject
    {
        [Header("Cross")]
        [SerializeField] private float _crossPulseDuration = 0.10f;

        [Header("Correct duck")]
        [SerializeField] private float _anticipationScale = 0.9f;
        [SerializeField] private float _anticipationDuration = 0.04f;
        [SerializeField] private float _impactScale = 1.15f;
        [SerializeField] private float _impactDuration = 0.12f;
        [SerializeField] private float _settleDuration = 0.08f;
        [SerializeField] private Color _successFlashColor = new Color(1f, 0.95f, 0.6f, 1f);

        [Header("Error")]
        [SerializeField] private float _shakeDuration = 0.18f;
        [SerializeField] private int _shakeVibrato = 12;
        [SerializeField] private Vector3 _shakeStrength = new Vector3(6f, 0f, 0f);
        [SerializeField] private float _backgroundTintDuration = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _blockedDarkenFactor = 0.55f;

        [Header("Wave")]
        [SerializeField] private float _waveFlashDuration = 0.12f;
        [SerializeField] private float _waveSettleDuration = 0.18f;
        [SerializeField] private float _groupWaveDelayStep = 0.03f;

        [Header("Grid appearance")]
        [SerializeField] private Color _introBaseColor = Color.white;
        [SerializeField] private float _introPopScale = 0.85f;
        [SerializeField] private float _introPopDuration = 0.18f;
        [SerializeField] private float _introFillDuration = 0.22f;
        [SerializeField] private float _introWaveDelayStep = 0.035f;

        [Header("Hint")]
        [SerializeField] private Color _hintAccentColor = new Color(1f, 0.85f, 0.3f, 0.85f);
        [SerializeField] private float _hintAccentPulseDuration = 0.5f;

        [Header("Hover")]
        [SerializeField] private Color _hoverColor = new Color(1f, 1f, 1f, 0.12f);
        [SerializeField] private float _hoverFadeDuration = 0.12f;

        [Header("Unavailable")]
        [SerializeField] private float _blockedShakeDuration = 0.10f;
        [SerializeField] private int _blockedShakeVibrato = 8;
        [SerializeField] private Vector3 _blockedShakeStrength = new Vector3(3f, 0f, 0f);

        public float CrossPulseDuration => _crossPulseDuration;

        public float AnticipationScale => _anticipationScale;
        public float AnticipationDuration => _anticipationDuration;
        public float ImpactScale => _impactScale;
        public float ImpactDuration => _impactDuration;
        public float SettleDuration => _settleDuration;
        public Color SuccessFlashColor => _successFlashColor;

        public float ShakeDuration => _shakeDuration;
        public int ShakeVibrato => _shakeVibrato;
        public Vector3 ShakeStrength => _shakeStrength;
        public float BackgroundTintDuration => _backgroundTintDuration;
        public float BlockedDarkenFactor => _blockedDarkenFactor;

        public float WaveFlashDuration => _waveFlashDuration;
        public float WaveSettleDuration => _waveSettleDuration;
        public float GroupWaveDelayStep => _groupWaveDelayStep;

        public Color IntroBaseColor => _introBaseColor;
        public float IntroPopScale => _introPopScale;
        public float IntroPopDuration => _introPopDuration;
        public float IntroFillDuration => _introFillDuration;
        public float IntroWaveDelayStep => _introWaveDelayStep;

        public Color HintAccentColor => _hintAccentColor;
        public float HintAccentPulseDuration => _hintAccentPulseDuration;

        public Color HoverColor => _hoverColor;
        public float HoverFadeDuration => _hoverFadeDuration;

        public float BlockedShakeDuration => _blockedShakeDuration;
        public int BlockedShakeVibrato => _blockedShakeVibrato;
        public Vector3 BlockedShakeStrength => _blockedShakeStrength;
    }
}
