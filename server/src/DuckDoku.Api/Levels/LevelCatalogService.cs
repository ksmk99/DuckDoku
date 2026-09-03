namespace DuckDoku.Api;

public class LevelCatalogService
{
    private LevelCatalog? _catalog;

    public LevelCatalog Catalog =>
        _catalog ?? throw new InvalidOperationException(
            "Level catalog has not been initialized.");

    public void Initialize(LevelCatalog catalog)
    {
        _catalog = catalog;
        _catalog.Initialize();
    }
}