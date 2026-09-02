using System;
using System.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class CatalogLevelSource : ILevelSource
    {
        private readonly LevelCatalogAsset _catalog;

        private int _nextIndex;

        public CatalogLevelSource(LevelCatalogAsset catalog)
        {
            _catalog = catalog;
        }

        public Task<PuzzleDefinition> GetNextPuzzleAsync()
        {
            if (_nextIndex >= _catalog.Levels.Length)
            {
                throw new InvalidOperationException("Каталог уровней пройден целиком.");
            }

            LevelRecord record = _catalog.Levels[_nextIndex];
            _nextIndex++;

            return Task.FromResult(ToDefinition(record));
        }

        private static PuzzleDefinition ToDefinition(LevelRecord record)
        {
            Cell[] solution = new Cell[record.Solution.Length];
            for (int i = 0; i < solution.Length; i++)
            {
                solution[i] = new Cell(record.Solution[i].row, record.Solution[i].column);
            }

            return new PuzzleDefinition(record.Size, record.Seed, record.Difficulty, record.Regions, solution);
        }
    }
}
