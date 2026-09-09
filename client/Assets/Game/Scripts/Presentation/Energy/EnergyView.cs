using System;
using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class EnergyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private TMP_Text _countdownText;

        public void SetValue(int value, int max)
        {
            _valueText.text = $"{value}";
        }

        public void SetCountdown(TimeSpan? remaining)
        {
            if (remaining is null)
            {
                _countdownText.text = "max";
                return;
            }
            
            _countdownText.text = remaining.Value.ToString(@"mm\:ss");
        }

        public void Pulse()
        {
        }
    }
}
