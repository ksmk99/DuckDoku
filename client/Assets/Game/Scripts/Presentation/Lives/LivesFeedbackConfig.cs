using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "LivesFeedbackConfig", menuName = "DuckDoku/Lives Feedback Config")]
    public class LivesFeedbackConfig : ScriptableObject
    {
        [Header("Спрайты")]
        [SerializeField] private Color _fullColor  = Color.white;
        [SerializeField] private Color _emptyColor = Color.black;

        [Header("Потеря жизни")]
        [SerializeField] private Vector3 _punchStrength = new Vector3(0.3f, 0.3f, 0f);
        [SerializeField] private float _punchDuration = 0.25f;
        [SerializeField] private int _punchVibrato = 8;

        public Color FullColor => _fullColor;
        public Color EmptyColor => _emptyColor;

        public Vector3 PunchStrength => _punchStrength;
        public float PunchDuration => _punchDuration;
        public int PunchVibrato => _punchVibrato;
    }
}
