using System;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelLauncher : ILevelLauncher
    {
        private readonly IStateMachine _stateMachine;
        private int _levelId = -1;

        public LevelLauncher(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public int GetLevelId()
        {
            if (_levelId == -1)
            {
                throw new Exception("Level is not set yet");
            }
            
            return _levelId;
        }
        
        public void LaunchLevel(int levelId)
        {
            _levelId = levelId;

            _stateMachine.TransitionTo(new GameplaySceneState()).Forget();
        }

        public void ReturnToMap()
        {
            _stateMachine.TransitionTo(new MetaSceneState()).Forget();
        }
    }
}