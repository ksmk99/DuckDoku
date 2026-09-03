namespace DuckDoku.App
{
    public class MetaSceneState: ISceneState
    {
        public string SceneName => "Meta";

        // Loading the scene itself is centralized in SceneStateMachine.TransitionTo.
        public void Enter() { }

        public void Exit() { }
    }
}