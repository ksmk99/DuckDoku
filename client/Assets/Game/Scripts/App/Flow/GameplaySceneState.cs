namespace DuckDoku.App
{
    public class GameplaySceneState : ISceneState
    {
        public string SceneName => "Gameplay";

        // Loading the scene itself is centralized in SceneStateMachine.TransitionTo.
        public void Enter() { }

        public void Exit() { }
    }
}
