using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class LevelSessionService : ILevelSessionService
    {
        private readonly ILevelsClient _levelsClient;

        public LevelSessionService(ILevelsClient levelsClient)
        {
            _levelsClient = levelsClient;
        }

        public async UniTask<string> StartLevelAsync(int levelId)
        {
            StartLevelResponse response = await _levelsClient.StartLevel(levelId);
            return response.sessionId;
        }

        public async UniTask<LevelResult> CompleteLevelAsync(int levelId, string sessionId, IReadOnlyList<Cell> placement)
        {
            CompleteLevelResponse response = await _levelsClient.CompleteLevel(levelId, sessionId, placement);
            return new LevelResult(response.stars, response.duration);
        }
    }
}
