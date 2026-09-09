using System;

namespace DuckDoku.App
{
    [Serializable]
    public class NextLevelResponse
    {
        public int nextLevelId;
        public int energy;
        public int energyMax;
        public long energyRefillMs;
        public LevelStarsEntry[] stars;
    }

    [Serializable]
    public class LevelStarsEntry
    {
        public int levelId;
        public int stars;
    }
}
