using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public readonly struct LevelToPlay
    {
        public int LevelId { get; }
        public PuzzleDefinition Puzzle { get; }

        public LevelToPlay(int levelId, PuzzleDefinition puzzle)
        {
            LevelId = levelId;
            Puzzle = puzzle;
        }
    }
}
