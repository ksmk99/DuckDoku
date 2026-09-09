using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public interface ILevelsClient
    {
        UniTask<NextLevelResponse> GetNextLevel(CancellationToken cancellationToken = default);

        UniTask<StartLevelResponse> StartLevel(int levelId, CancellationToken cancellationToken = default);

        UniTask<CompleteLevelResponse> CompleteLevel(int levelId, string sessionId,
            IReadOnlyList<Cell> placement, CancellationToken cancellationToken = default);

        UniTask<UseHintResponse> UseHint(int levelId, string sessionId, CancellationToken cancellationToken = default);
    }
}
