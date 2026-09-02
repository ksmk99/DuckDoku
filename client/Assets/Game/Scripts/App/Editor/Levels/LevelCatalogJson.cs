using System;

namespace DuckDoku.App
{
    [Serializable]
    public class LevelCatalogJson
    {
        public int catalogVersion;
        public LevelEntryJson[] levels;
    }
}
