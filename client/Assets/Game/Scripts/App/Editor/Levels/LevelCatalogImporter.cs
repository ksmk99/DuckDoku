using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DuckDoku.App
{
    public static class LevelCatalogImporter
    {
        private const string JsonRelativePath = "../../content/levels/catalog.json";
        private const string AssetPath = "Assets/Game/Configs/LevelCatalog.asset";

        [MenuItem("DuckDoku/Bake Level Catalog")]
        public static void Bake()
        {
            string jsonPath = Path.GetFullPath(Path.Combine(Application.dataPath, JsonRelativePath));

            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"LevelCatalogImporter: файл не найден — {jsonPath}");
                return;
            }

            string json = File.ReadAllText(jsonPath);
            LevelCatalogJson raw = JsonUtility.FromJson<LevelCatalogJson>(json);

            if (raw == null || raw.levels == null)
            {
                Debug.LogError("LevelCatalogImporter: не удалось разобрать catalog.json.");
                return;
            }

            LevelCatalogAsset asset = AssetDatabase.LoadAssetAtPath<LevelCatalogAsset>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<LevelCatalogAsset>();
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            asset.CatalogVersion = raw.catalogVersion;
            asset.Levels = raw.levels.Select(ToRecord).ToArray();

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            Debug.Log($"LevelCatalogImporter: испечено {asset.Levels.Length} уровней, catalogVersion={asset.CatalogVersion}.");
        }

        private static LevelRecord ToRecord(LevelEntryJson entry)
        {
            return new LevelRecord
            {
                Id = entry.id,
                Size = entry.size,
                Seed = entry.seed,
                Difficulty = entry.difficulty,
                Regions = entry.regions,
                Solution = entry.solution,
                BaseReward = entry.baseReward,
            };
        }
    }
}
