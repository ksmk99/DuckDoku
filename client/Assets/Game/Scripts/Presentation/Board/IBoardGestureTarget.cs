namespace DuckDoku.Presentation
{
    public interface IBoardGestureTarget
    {
        bool CanEdit(int row, int column);

        bool IsMarked(int row, int column);

        void Mark(int row, int column, bool marked);

        void PlaceDuck(int row, int column);
    }
}
