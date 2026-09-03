using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IStateMachine
    {
        event Action<ISceneState> Changed;
        ISceneState CurrentState { get; }
        UniTask TransitionTo(ISceneState nextState);
    }
}