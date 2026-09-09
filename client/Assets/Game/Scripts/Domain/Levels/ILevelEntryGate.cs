namespace DuckDoku.Domain
{
    public interface ILevelEntryGate
    {
        bool TryStart(int levelId);
    }
}
