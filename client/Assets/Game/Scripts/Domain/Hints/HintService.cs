using System;
using System.Collections.Generic;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public class HintService
    {
        private static readonly Random Random = new Random();

        private BoardState _board;
        private PuzzleDefinition _definition;

        public Cell? ActiveHint { get; private set; }

        public event Action Changed;

        public void Attach(BoardState board, PuzzleDefinition definition)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            Detach();

            _board = board;
            _definition = definition;
            _board.CellChanged += OnCellChanged;
        }

        public void Detach()
        {
            if (_board == null)
            {
                return;
            }

            _board.CellChanged -= OnCellChanged;
            _board = null;
            _definition = null;

            ClearHint();
        }

        public bool TryRequestHint()
        {
            if (_board == null)
            {
                return false;
            }

            List<Cell> candidates = null;

            foreach (Cell cell in _definition.Solution)
            {
                if (_board.GetCellState(cell.Row, cell.Column) != CellState.Duck)
                {
                    (candidates ??= new List<Cell>()).Add(cell);
                }
            }

            if (candidates == null)
            {
                return false;
            }

            ActiveHint = candidates[Random.Next(candidates.Count)];
            Changed?.Invoke();

            return true;
        }

        private void OnCellChanged(int row, int column)
        {
            if (ActiveHint == null)
            {
                return;
            }

            Cell hint = ActiveHint.Value;

            if (hint.Row == row && hint.Column == column && _board.GetCellState(row, column) == CellState.Duck)
            {
                ClearHint();
            }
        }

        private void ClearHint()
        {
            if (ActiveHint == null)
            {
                return;
            }

            ActiveHint = null;
            Changed?.Invoke();
        }
    }
}
