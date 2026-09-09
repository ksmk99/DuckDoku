using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace DuckDoku.App
{
    public class SceneStateMachine : IStateMachine
    {
        private readonly Dictionary<string, ISceneState> _additiveStates = new Dictionary<string, ISceneState>();

        private bool _isTransitioning;

        public SceneStateMachine(ISceneState initialState)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            CurrentState = initialState;
        }

        public event Action<ISceneState> Changed;

        public ISceneState CurrentState { get; private set; }

        public async UniTask TransitionTo(ISceneState nextState)
        {
            if (nextState == null)
            {
                throw new ArgumentNullException(nameof(nextState));
            }

            if (_isTransitioning || nextState.SceneName == CurrentState.SceneName)
            {
                return;
            }

            _isTransitioning = true;

            CurrentState.Exit();

            foreach (ISceneState additiveState in _additiveStates.Values)
            {
                additiveState.Exit();
            }

            _additiveStates.Clear();

            await SceneManager.LoadSceneAsync(nextState.SceneName, LoadSceneMode.Single).ToUniTask();

            nextState.Enter();
            CurrentState = nextState;
            _isTransitioning = false;

            Changed?.Invoke(nextState);
        }

        public async UniTask LoadAdditive(ISceneState overlayState)
        {
            if (overlayState == null)
            {
                throw new ArgumentNullException(nameof(overlayState));
            }

            if (_additiveStates.ContainsKey(overlayState.SceneName))
            {
                return;
            }

            _additiveStates[overlayState.SceneName] = overlayState;

            await SceneManager.LoadSceneAsync(overlayState.SceneName, LoadSceneMode.Additive).ToUniTask();

            overlayState.Enter();
        }

        public async UniTask UnloadAdditive(ISceneState overlayState)
        {
            if (overlayState == null)
            {
                throw new ArgumentNullException(nameof(overlayState));
            }

            if (!_additiveStates.Remove(overlayState.SceneName))
            {
                return;
            }

            overlayState.Exit();

            await SceneManager.UnloadSceneAsync(overlayState.SceneName).ToUniTask();
        }
    }
}
