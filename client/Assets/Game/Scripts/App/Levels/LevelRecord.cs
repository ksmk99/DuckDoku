using System;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    [Serializable]
    public struct LevelRecord
    {
        public int Id;
        public int Size;
        public long Seed;
        public PuzzleDifficulty Difficulty;
        public int[] Regions;
        public SerializedCell[] Solution;
        public int BaseReward;
    }
}
