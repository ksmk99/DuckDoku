using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace DuckDoku.Domain
{
    public interface ILevelMapSource
    {
        UniTask<IReadOnlyList<LevelSummary>> GetLevelsAsync();

        UniTask<int> GetNextLevelIdAsync();
    }
}