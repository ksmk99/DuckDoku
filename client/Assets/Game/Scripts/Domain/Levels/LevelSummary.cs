namespace DuckDoku.Domain
{
    public readonly struct LevelSummary
    {
        public int LevelId { get; }
        public LevelStatus Status { get; }
        public int Stars { get; }

        public LevelSummary(int levelId, LevelStatus status, int stars)
        {
            LevelId = levelId;
            Status = status;
            Stars = stars;
        }
    }
}