using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueText;

        public void SetValue(int value)
        {
            _valueText.text = $"{value}";
        }
    }
}
