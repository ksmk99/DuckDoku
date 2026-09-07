using System;
using System.Collections.Generic;
using DuckDoku.Presentation;

namespace DuckDoku.Tests
{
    public enum FakeCell
    {
        Empty,
        Cross,
        Locked
    }

    public class FakeGestureBoard : IBoardGestureTarget
    {
        private readonly FakeCell[,] _cells;

        public FakeGestureBoard(int size = 8)
        {
            _cells = new FakeCell[size, size];
        }

        public List<string> Log { get; } = new List<string>();

        public void Set(int row, int column, FakeCell cell)
        {
            _cells[row, column] = cell;
        }

        public FakeCell At(int row, int column)
        {
            return _cells[row, column];
        }

        public bool CanEdit(int row, int column)
        {
            return _cells[row, column] != FakeCell.Locked;
        }

        public bool IsMarked(int row, int column)
        {
            return _cells[row, column] == FakeCell.Cross;
        }

        public void Mark(int row, int column, bool marked)
        {
            if (_cells[row, column] == FakeCell.Locked)
            {
                throw new InvalidOperationException($"Запись в залоченную клетку {row}:{column}.");
            }

            _cells[row, column] = marked ? FakeCell.Cross : FakeCell.Empty;

            Log.Add($"mark {row}:{column}={(marked ? "x" : "-")}");
        }

        public void PlaceDuck(int row, int column)
        {
            Log.Add($"duck {row}:{column}");
        }
    }
}
