namespace DuckDoku.Api;

public sealed class LevelCatalog
{
    public int CatalogVersion { get; init; }
    public IReadOnlyList<LevelRecord> Levels { get; init; } = [];
    
    private Dictionary<int, LevelRecord> _catalog;

    public void Initialize()
    {
        if (_catalog == null)
        {
            _catalog = Levels.ToDictionary(level => level.Id, level => level);
        }
    }

    public LevelRecord GetLevel(int levelId)
    {
        if (_catalog == null)
        {
            _catalog = Levels.ToDictionary(level => level.Id, level => level);
        }

        if (_catalog.TryGetValue(levelId, out var level))
        {
            return level;
        }
        else
        {
            throw  new KeyNotFoundException($"Level {levelId} not found");
        }
    }

    public bool HasLevel(int levelId)
    {
        if (_catalog == null)
        {
            _catalog = Levels.ToDictionary(level => level.Id, level => level);
        }
        
        return _catalog.TryGetValue(levelId, out var level);
    }
}