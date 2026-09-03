namespace DuckDoku.Api;

public class LevelCatalogInitializer : IHostedService
{
    private readonly ILevelCatalogLoader _loader;
    private readonly LevelCatalogService _catalogService;

    public LevelCatalogInitializer(
        ILevelCatalogLoader loader,
        LevelCatalogService catalogService)
    {
        _loader = loader;
        _catalogService = catalogService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var catalog = await _loader.LoadAsync(cancellationToken);

        _catalogService.Initialize(catalog);

        Console.WriteLine(
            $"Level catalog loaded: {catalog.Levels.Count} levels (v{catalog.CatalogVersion}).");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}