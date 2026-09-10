using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class DucksCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _countText;

        public void SetCount(int placed, int total)
        {
            _countText.text = $"{placed.ToString()}/{total.ToString()}";
        }
    }
}
