namespace DuckDoku.Domain
{
    public readonly struct LevelSummary
    {
        public int LevelId { get; }
        public LevelStatus Status { get; }
        
        public LevelSummary(int levelId, LevelStatus status)
        {
            LevelId = levelId;
            Status = status;
        }
    }
}