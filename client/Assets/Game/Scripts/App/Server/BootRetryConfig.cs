using UnityEngine;

namespace DuckDoku.App
{
    [CreateAssetMenu(fileName = "BootRetryConfig", menuName = "DuckDoku/Boot Retry Config")]
    public class BootRetryConfig : ScriptableObject
    {
        [Header("Окно попыток")]
        [SerializeField] private float _maxRetryWindowSeconds = 90f;

        [Header("Backoff")]
        [SerializeField] private float _baseRetryDelaySeconds = 2f;
        [SerializeField] private float _maxRetryDelaySeconds = 10f;

        public float MaxRetryWindowSeconds => _maxRetryWindowSeconds;
        public float BaseRetryDelaySeconds => _baseRetryDelaySeconds;
        public float MaxRetryDelaySeconds => _maxRetryDelaySeconds;
    }
}
