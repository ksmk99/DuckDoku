using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public interface ILevelSessionService
    {
        UniTask<string> StartLevelAsync(int levelId);

        UniTask<LevelResult> CompleteLevelAsync(int levelId, string sessionId, IReadOnlyList<Cell> placement);
    }
}
