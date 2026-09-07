using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class BoardSessionFactory : IBoardSessionFactory
    {
        public BoardSession Create(PuzzleDefinition definition)
        {
            BoardState board = new BoardState(definition);
            MistakeTracker mistakes = new MistakeTracker();

            return new BoardSession(definition, board, mistakes);
        }
    }
}
