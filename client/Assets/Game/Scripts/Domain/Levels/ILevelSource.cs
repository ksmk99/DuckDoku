using Cysharp.Threading.Tasks;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public interface ILevelSource
    {
        UniTask<LevelToPlay> GetPuzzleByLevel(int levelId);
    }
}
