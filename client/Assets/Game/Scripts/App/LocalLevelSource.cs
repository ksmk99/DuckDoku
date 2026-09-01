using System.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;
using UnityEngine;

namespace DuckDoku.App
{
    public class LocalLevelSource : ILevelSource
    {
        public Task<PuzzleDefinition> GetPuzzleAsync(int size, PuzzleDifficulty difficulty)
        {
            var definition = PuzzleGenerator.Generate(size, difficulty, Random.Range(0, 1000));
            return Task.FromResult(definition);
        }
    }
}