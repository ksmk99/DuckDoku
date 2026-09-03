namespace DuckDoku.Domain
{
    public interface ILevelLauncher
    {
        int GetLevelId();
        void LaunchLevel(int levelId);
        void ReturnToMap();
    }
}