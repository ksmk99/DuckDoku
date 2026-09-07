using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public interface IBoardSessionFactory
    {
        BoardSession Create(PuzzleDefinition definition);
    }
}
