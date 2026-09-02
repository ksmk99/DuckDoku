using System.Text.Json;

namespace DuckDoku.LevelBaker.Core
{
    public static class CatalogWriter
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static void Write(string path, int catalogVersion, IReadOnlyList<CatalogEntry> entries)
        {
            var document = new CatalogDocument
            {
                CatalogVersion = catalogVersion,
                Levels = entries.OrderBy(entry => entry.Id).ToArray(),
            };

            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, JsonSerializer.Serialize(document, Options));
        }

        private class CatalogDocument
        {
            public int CatalogVersion { get; init; }
            public CatalogEntry[] Levels { get; init; } = Array.Empty<CatalogEntry>();
        }
    }
}
