using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;

namespace DuckDoku.App
{
    public class LevelMapSource : ILevelMapSource
    {
        private readonly LevelCatalogAsset _catalog;
        private readonly ILevelsClient _levelsClient;
        private readonly IEnergyService _energyService;

        public LevelMapSource(LevelCatalogAsset catalog, ILevelsClient levelsClient, IEnergyService energyService)
        {
            _catalog = catalog;
            _levelsClient = levelsClient;
            _energyService = energyService;
        }

        public async UniTask<int> GetNextLevelIdAsync()
        {
            NextLevelResponse response = await _levelsClient.GetNextLevel();

            _energyService.Apply(response.energy, response.energyMax, response.energyRefillMs);

            return response.nextLevelId;
        }

        public async UniTask<IReadOnlyList<LevelSummary>> GetLevelsAsync()
        {
            NextLevelResponse response = await _levelsClient.GetNextLevel();

            _energyService.Apply(response.energy, response.energyMax, response.energyRefillMs);

            int nextLevelId = response.nextLevelId;
            Dictionary<int, int> starsByLevelId = ToStarsMap(response.stars);

            LevelRecord[] levels = _catalog.Levels.OrderBy(record => record.Id).ToArray();
            var summaries = new LevelSummary[levels.Length];

            for (int i = 0; i < levels.Length; i++)
            {
                int levelId = levels[i].Id;
                int stars = starsByLevelId.TryGetValue(levelId, out int value) ? value : 0;

                summaries[i] = new LevelSummary(levelId, ToStatus(levelId, nextLevelId), stars);
            }

            return summaries;
        }

        private static Dictionary<int, int> ToStarsMap(LevelStarsEntry[] entries)
        {
            var map = new Dictionary<int, int>();

            if (entries == null)
            {
                return map;
            }

            foreach (LevelStarsEntry entry in entries)
            {
                map[entry.levelId] = entry.stars;
            }

            return map;
        }

        private static LevelStatus ToStatus(int levelId, int nextLevelId)
        {
            if (nextLevelId == LevelMapPolicy.NoNextLevel)
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
