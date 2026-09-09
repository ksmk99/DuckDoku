using System;
using TMPro;
using UnityEngine;

namespace DuckDoku.Presentation
{
    public class LevelTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        public void SetElapsed(TimeSpan elapsed)
        {
            _timerText.text = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
        }
    }
}
