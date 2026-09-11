using System;
using System.Collections.Generic;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public class HintService : ISessionAttachable
    {
        private static readonly Random Random = new Random();

        private BoardState _board;
        private IReadOnlyCollection<Cell> _solution;

        public Cell? ActiveHint { get; private set; }

        public event Action Changed;

        public void Attach(BoardSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            Detach();

            _board = session.Board;
            _solution = session.Solution;
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
            _solution = null;

            ClearHint();
        }

        public bool TryRequestHint()
        {
            if (_board == null)
            {
                return false;
            }

            List<Cell> candidates = null;

            foreach (Cell cell in _solution)
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
