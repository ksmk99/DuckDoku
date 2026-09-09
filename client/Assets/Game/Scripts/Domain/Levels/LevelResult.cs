namespace DuckDoku.Domain
{
    public readonly struct LevelResult
    {
        public int Stars { get; }
        public long DurationMs { get; }
        public int Coins { get; }

        public LevelResult(int stars, long durationMs, int coins)
        {
            Stars = stars;
            DurationMs = durationMs;
            Coins = coins;
        }
    }
}
