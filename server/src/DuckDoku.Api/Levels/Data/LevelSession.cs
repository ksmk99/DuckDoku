namespace DuckDoku.Api;

public class LevelSession
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public int LevelId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? ClaimedAt { get; set; }
}