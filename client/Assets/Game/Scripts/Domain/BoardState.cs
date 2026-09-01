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

        public event Action Changed;
        public event Action Solved;

        public int Size => _definition.Size;

        public bool IsSolved { get; private set; }

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

        public void Toggle(int row, int column)
        {
            int index = Index(row, column);

            if (IsSolved)
            {
                return;
            }

            _cells[index] = _cells[index].Next();

            Refresh();

            Changed?.Invoke();

            if (IsSolved)
            {
                Solved?.Invoke();
            }
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
                    nameof(row), row, $"Строка должна быть от 0 до {Size - 1}.");
            }

            if (column < 0 || column >= Size)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(column), column, $"Колонка должна быть от 0 до {Size - 1}.");
            }

            return row * Size + column;
        }
    }
}
