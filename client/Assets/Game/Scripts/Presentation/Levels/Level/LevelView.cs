using System;
using DuckDoku.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class LevelView : MonoBehaviour
    { 
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text[] _levelTexts;
        [Space]
        [SerializeField] private LevelViewState[] _states;
        [Space]
        [SerializeField] private LevelStarView[] _starViews;

        public event Action Onclick;

        public void Setup(int levelId, LevelStatus status, int stars)
        {
            foreach (TMP_Text text in _levelTexts)
            {
                text.text = levelId.ToString();
            }

            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].StateObject.SetActive(status == _states[i].Status);
            }

            for (int i = 0; i < _starViews.Length; i++)
            {
                LevelStarState state = i < stars ? LevelStarState.Earned : LevelStarState.Unearned;
                _starViews[i].SetState(state);
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Click);
        }

        private void Click()
        {
            Onclick?.Invoke();
        }
    }

    [Serializable]
    public struct LevelViewState
    {
        public LevelStatus Status;
        public GameObject StateObject;
    }
}