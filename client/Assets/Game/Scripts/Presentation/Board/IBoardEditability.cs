namespace DuckDoku.Presentation
{
    public interface IBoardEditability
    {
        bool CanEdit(int row, int column);
    }
}
