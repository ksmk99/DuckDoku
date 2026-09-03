namespace DuckDoku.App
{
    public class BootSceneState : ISceneState
    {
        public string SceneName => "Boot";

        // Loading the scene itself is centralized in SceneStateMachine.TransitionTo.
        public void Enter() { }

        public void Exit() { }
    }
}
