namespace DuckDoku.Domain
{
    public interface ISessionAttachable
    {
        void Attach(BoardSession session);
        void Detach();
    }
}
