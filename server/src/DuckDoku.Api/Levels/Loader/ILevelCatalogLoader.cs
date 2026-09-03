namespace DuckDoku.Api;

public interface ILevelCatalogLoader
{
    Task<LevelCatalog> LoadAsync(CancellationToken cancellationToken);
}