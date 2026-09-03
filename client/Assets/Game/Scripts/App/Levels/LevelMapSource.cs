using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelMapSource : ILevelMapSource
    {
        private const int NoNextLevel = -1;

        private readonly LevelCatalogAsset _catalog;
        private readonly ILevelsClient _levelsClient;

        public LevelMapSource(LevelCatalogAsset catalog, ILevelsClient levelsClient)
        {
            _catalog = catalog;
            _levelsClient = levelsClient;
        }

        public async UniTask<IReadOnlyList<LevelSummary>> GetLevelsAsync()
        {
            NextLevelResponse response = await _levelsClient.GetNextLevel();
            int nextLevelId = response.nextLevelId;

            LevelRecord[] levels = _catalog.Levels.OrderBy(record => record.Id).ToArray();
            var summaries = new LevelSummary[levels.Length];

            for (int i = 0; i < levels.Length; i++)
            {
                summaries[i] = new LevelSummary(levels[i].Id, ToStatus(levels[i].Id, nextLevelId));
            }

            return summaries;
        }

        private static LevelStatus ToStatus(int levelId, int nextLevelId)
        {
            if (nextLevelId == NoNextLevel)
            {
                return LevelStatus.Completed;
            }

            if (levelId < nextLevelId)
            {
                return LevelStatus.Completed;
            }

            if (levelId == nextLevelId)
            {
                return LevelStatus.Unlocked;
            }

            return LevelStatus.Locked;
        }
    }
}
