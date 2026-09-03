namespace DuckDoku.Domain
{
    public readonly struct LevelResult
    {
        public int Stars { get; }
        public long DurationMs { get; }

        public LevelResult(int stars, long durationMs)
        {
            Stars = stars;
            DurationMs = durationMs;
        }
    }
}
