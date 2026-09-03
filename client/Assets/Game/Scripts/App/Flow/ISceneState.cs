namespace DuckDoku.App
{
    public interface ISceneState
    {
        string SceneName { get; }

        void Enter();
        void Exit();
    }
}
