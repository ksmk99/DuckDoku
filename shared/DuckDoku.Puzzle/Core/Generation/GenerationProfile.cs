using System;

namespace DuckDoku.Puzzle
{
    public readonly struct GenerationProfile
    {
        public int ZoneCount { get; }
        public int MinZoneSize { get; }
        public int MaxZoneSize { get; }
        
        private GenerationProfile(int zoneCount, int minZoneSize, int maxZoneSize)
        {
            ZoneCount = zoneCount;
            MinZoneSize = minZoneSize;
            MaxZoneSize = maxZoneSize;
        }

        public static GenerationProfile For(int size, PuzzleDifficulty difficulty)
        {
            int limit = size / 3;

            switch (difficulty)
            {
                case PuzzleDifficulty.Easy:
                {
                    return new GenerationProfile(limit, 1, 2);
                }
                case PuzzleDifficulty.Normal:
                {
                    return new GenerationProfile(limit, 1, 3);
                }
                case PuzzleDifficulty.Hard:
                {
                    return new GenerationProfile(Math.Max(1, limit - 1), 2, 3);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(difficulty), difficulty, "Неизвестный класс сложности.");
                }
            }
        }

        public int[] RollCaps(int size, PuzzleRandom rng)
        {
            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }

            int[] caps = new int[size];
            for (int i = 0; i < size; i++)
            {
                caps[i] = RegionGrower.NoCap;
            }

            int[] order = new int[size];
            for (int i = 0; i < size; i++)
            {
                order[i] = i;
            }

            rng.Shuffle(order);

            int span = MaxZoneSize - MinZoneSize + 1;

            for (int i = 0; i < ZoneCount && i < size; i++)
            {
                caps[order[i]] = MinZoneSize + rng.NextInt(span);
            }

            return caps;
        }
    }
}
