using System.Threading.Tasks;
using DuckDoku.Puzzle;

namespace DuckDoku.Domain
{
    public interface ILevelSource
    {
        Task<PuzzleDefinition> GetNextPuzzleAsync();
    }
}
