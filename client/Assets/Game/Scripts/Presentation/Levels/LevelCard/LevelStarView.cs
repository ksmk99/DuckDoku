using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public enum LevelStarState
    {
        Unearned,
        Earned
    }

    public class LevelStarView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Color _earnedColor = Color.white;
        [SerializeField] private Color _unearnedColor = Color.gray;

        public void SetState(LevelStarState state)
        {
            _icon.color = state == LevelStarState.Earned ? _earnedColor : _unearnedColor;
        }
    }
}
