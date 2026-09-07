using System;
using System.Collections.Generic;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public class BoardSession : IDisposable
    {
        private readonly HashSet<Cell> _solution;

        private bool _isActive = true;

        public BoardSession(PuzzleDefinition definition, BoardState board, MistakeTracker mistakes)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (mistakes == null)
            {
                throw new ArgumentNullException(nameof(mistakes));
            }

            _solution = new HashSet<Cell>(definition.Solution);

            Board = board;
            Mistakes = mistakes;
        }

        public event Action<Cell> DuckRejected;

        public BoardState Board { get; }
        public MistakeTracker Mistakes { get; }

        public void SetMark(int row, int column, bool marked)
        {
            if (!_isActive)
            {
                return;
            }

            Board.SetMark(row, column, marked);
        }

        public bool TryPlaceDuck(int row, int column)
        {
            if (!_isActive || !Board.CanEdit(row, column))
            {
                return false;
            }

            Cell cell = new Cell(row, column);

            if (_solution.Contains(cell))
            {
                Board.PlaceDuck(row, column);
                return true;
            }

            Board.Block(row, column);

            DuckRejected?.Invoke(cell);
            Mistakes.RegisterMistake();

            return false;
        }

        public void Dispose()
        {
            _isActive = false;
        }
    }
}
