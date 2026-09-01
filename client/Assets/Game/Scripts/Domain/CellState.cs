using System;

namespace DuckDoku.Domain
{
    public enum CellState
    {
        Empty,
        Cross,
        Duck
    }
    
    public static class CellStateExtensions
    {
        private static readonly CellState[] Values =
            (CellState[])Enum.GetValues(typeof(CellState));

        public static CellState Next(this CellState state)
        {
            int index = Array.IndexOf(Values, state);
            return Values[(index + 1) % Values.Length];
        }
    }
}