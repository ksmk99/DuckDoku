using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class ExitLevelView : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;

        public event Action ExitRequested;

        private void Awake()
        {
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnExitClicked()
        {
            ExitRequested?.Invoke();
        }
    }
}
