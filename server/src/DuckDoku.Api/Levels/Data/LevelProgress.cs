namespace DuckDoku.Api;

public class LevelProgress
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public int LevelId { get; set; }
    public int BestResult { get; set; }
    public long BestTimeMs { get; set; }
    public DateTime CompletedAt { get; set; }
}