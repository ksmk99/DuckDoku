using System;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    [Serializable]
    public class LevelEntryJson
    {
        public int id;
        public int size;
        public long seed;
        public PuzzleDifficulty difficulty;
        public int[] regions;
        public SerializedCell[] solution;
        public int baseReward;
    }
}
