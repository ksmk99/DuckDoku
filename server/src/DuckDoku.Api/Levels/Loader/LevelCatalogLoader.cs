using System.Text.Json;

namespace DuckDoku.Api;

public class LevelCatalogLoader : ILevelCatalogLoader
{
    public async Task<LevelCatalog> LoadAsync(CancellationToken cancellationToken)
    {
        string path = Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..",
            "content", "levels", "catalog.json"));

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Level catalog file was not found. Run tools/level-baker first.", path);
        }

        await using FileStream stream = File.OpenRead(path);

        LevelCatalog? catalog;

        try
        {
            catalog = JsonSerializer.Deserialize<LevelCatalog>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize level catalog: {path}",
                exception);
        }

        return catalog ?? throw new InvalidOperationException(
                   "Failed to deserialize level catalog.");
    }
}