using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "BoardInputConfig", menuName = "DuckDoku/Board Input Config")]
    public class BoardInputConfig : ScriptableObject
    {
        [Header("Double click")]
        [SerializeField] private float _doubleTapSeconds = 0.22f;

        [Header("Stuck-gesture guard")]
        [SerializeField] private float _staleGestureTimeoutSeconds = 1.5f;

        public float DoubleTapSeconds => _doubleTapSeconds;
        public float StaleGestureTimeoutSeconds => _staleGestureTimeoutSeconds;
    }
}
