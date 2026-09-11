using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "MainMenuFeedbackConfig", menuName = "DuckDoku/Main Menu Feedback Config")]
    public class MainMenuFeedbackConfig : ScriptableObject
    {
        [Header("Level start refusal")]
        [SerializeField] private float _shakeDuration = 0.3f;
        [SerializeField] private Vector3 _shakeStrength = new Vector3(12f, 0f, 0f);
        [SerializeField] private int _shakeVibrato = 10;

        public float ShakeDuration => _shakeDuration;
        public Vector3 ShakeStrength => _shakeStrength;
        public int ShakeVibrato => _shakeVibrato;
    }
}
