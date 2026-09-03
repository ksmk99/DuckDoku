using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace DuckDoku.App
{
    public class SceneStateMachine : IStateMachine
    {
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

            await SceneManager.LoadSceneAsync(nextState.SceneName, LoadSceneMode.Single).ToUniTask();

            nextState.Enter();
            CurrentState = nextState;
            _isTransitioning = false;

            Changed?.Invoke(nextState);
        }
    }
}
