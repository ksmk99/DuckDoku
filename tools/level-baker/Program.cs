using DuckDoku.LevelBaker.Core;
using DuckDoku.Puzzle;

const int CatalogVersion = 1;
const long MasterSeed = 02092026;

string outputPath = args.Length > 0 ? args[0] : "content/levels/catalog.json";

try
{
    var random = new PuzzleRandom(MasterSeed);
    var catalog = new List<CatalogEntry>();
    int nextId = 1;

    foreach (ProgressionBucket bucket in ProgressionPlan.Buckets)
    {
        catalog.AddRange(LevelBaking.BakeBucket(bucket, random, ref nextId));
    }

    CatalogWriter.Write(outputPath, CatalogVersion, catalog);

    Console.WriteLine($"level-baker: испечено {catalog.Count} уровней -> {outputPath}");
}
catch (InvalidOperationException exception)
{
    Console.Error.WriteLine($"level-baker: {exception.Message}");
    Environment.Exit(1);
}
