using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class LevelSessionService : ILevelSessionService
    {
        private readonly ILevelsClient _levelsClient;
        private readonly IEnergyService _energyService;

        public LevelSessionService(ILevelsClient levelsClient, IEnergyService energyService)
        {
            _levelsClient = levelsClient;
            _energyService = energyService;
        }

        public async UniTask<string> StartLevelAsync(int levelId)
        {
            StartLevelResponse response = await _levelsClient.StartLevel(levelId);

            _energyService.Apply(response.energy, response.energyMax, response.energyRefillMs);

            return response.sessionId;
        }

        public async UniTask<LevelResult> CompleteLevelAsync(int levelId, string sessionId, IReadOnlyList<Cell> placement)
        {
            CompleteLevelResponse response = await _levelsClient.CompleteLevel(levelId, sessionId, placement);
            return new LevelResult(response.stars, response.duration);
        }
    }
}
