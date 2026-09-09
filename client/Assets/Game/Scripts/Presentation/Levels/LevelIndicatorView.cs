using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LevelIndicatorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;

        public void SetLevel(int levelId)
        {
            _levelText.text = $"Level {levelId.ToString()}";
        }
    }
}
