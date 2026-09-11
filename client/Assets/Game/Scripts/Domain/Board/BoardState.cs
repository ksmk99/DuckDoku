using System;
using System.Collections.Generic;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public class BoardState
    {
        private readonly PuzzleDefinition _definition;
        private readonly CellState[] _cells;
        private readonly bool[] _conflicts;
        private readonly List<Cell> _ducks;

        public BoardState(PuzzleDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _definition = definition;
            _cells = new CellState[definition.Size * definition.Size];
            _conflicts = new bool[_cells.Length];
            _ducks = new List<Cell>(definition.Size);
        }

        public event Action<int, int> CellChanged;
        public event Action<Cell[]> Solved;

        public int Size => _definition.Size;

        public bool IsSolved { get; private set; }

        public IReadOnlyList<Cell> Ducks => _ducks;

        public int RegionAt(int row, int column)
        {
            return _definition.RegionAt(row, column);
        }

        public CellState GetCellState(int row, int column)
        {
            return _cells[Index(row, column)];
        }

        public bool HasConflict(int row, int column)
        {
            return _conflicts[Index(row, column)];
        }

        public bool CanEdit(int row, int column)
        {
            return !IsSolved && !_cells[Index(row, column)].IsLocked();
        }

        public void SetMark(int row, int column, bool marked)
        {
            int index = Index(row, column);

            if (!CanEdit(row, column))
            {
                return;
            }

            CellState next = marked ? CellState.Cross : CellState.Empty;

            if (_cells[index] == next)
            {
                return;
            }

            _cells[index] = next;

            CellChanged?.Invoke(row, column);
        }

        public void PlaceDuck(int row, int column)
        {
            int index = Index(row, column);

            if (!CanEdit(row, column))
            {
                return;
            }

            _cells[index] = CellState.Duck;

            Refresh();

            CellChanged?.Invoke(row, column);

            if (IsSolved)
            {
                Solved?.Invoke(_ducks.ToArray());
            }
        }

        public void Block(int row, int column)
        {
            int index = Index(row, column);

            if (!CanEdit(row, column))
            {
                return;
            }

            _cells[index] = CellState.Blocked;

            CellChanged?.Invoke(row, column);
        }

        private void Refresh()
        {
            _ducks.Clear();

            for (int row = 0; row < Size; row++)
            {
                for (int column = 0; column < Size; column++)
                {
                    if (_cells[row * Size + column] == CellState.Duck)
                    {
                        _ducks.Add(new Cell(row, column));
                    }
                }
            }

            Array.Clear(_conflicts, 0, _conflicts.Length);

            IReadOnlyList<RuleViolation> violations = PuzzleRules.FindViolations(_definition, _ducks);

            for (int i = 0; i < violations.Count; i++)
            {
                RuleViolation violation = violations[i];

                _conflicts[Index(violation.CellOne.Row, violation.CellOne.Column)] = true;
                _conflicts[Index(violation.CellTwo.Row, violation.CellTwo.Column)] = true;
            }

            IsSolved = _ducks.Count == Size && violations.Count == 0;
        }

        private int Index(int row, int column)
        {
            if (row < 0 || row >= Size)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(row), row, $"Row must be between 0 and {Size - 1}.");
            }

            if (column < 0 || column >= Size)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(column), column, $"Column must be between 0 and {Size - 1}.");
            }

            return row * Size + column;
        }
    }
}
