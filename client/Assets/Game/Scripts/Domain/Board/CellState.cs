namespace DuckDoku.Domain
{
    public enum CellState
    {
        Empty,
        Cross,
        Duck,
        Blocked
    }

    public static class CellStateExtensions
    {
        public static bool IsLocked(this CellState state)
        {
            return state == CellState.Duck || state == CellState.Blocked;
        }
    }
}
