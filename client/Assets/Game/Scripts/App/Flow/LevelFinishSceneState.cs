namespace DuckDoku.App
{
    public class LevelFinishSceneState : ISceneState
    {
        public string SceneName => "LevelFinish";

        // Loading the scene itself is centralized in SceneStateMachine.
        public void Enter() { }

        public void Exit() { }
    }
}
