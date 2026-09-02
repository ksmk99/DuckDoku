using UnityEngine;

namespace DuckDoku.App
{
    [CreateAssetMenu(fileName = "LevelCatalogAsset", menuName = "DuckDoku/LevelCatalogAsset")]
    public class LevelCatalogAsset : ScriptableObject
    {
        public int CatalogVersion;
        public LevelRecord[] Levels;
    }
}
