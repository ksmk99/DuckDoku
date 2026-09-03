using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DuckDoku.Domain;
using DuckDoku.Puzzle;

namespace DuckDoku.App
{
    public class CatalogLevelSource : ILevelSource
    {
        private readonly LevelCatalogAsset _catalog;
        
        public CatalogLevelSource(LevelCatalogAsset catalog)
        {
            _catalog = catalog;
        }
        
        public UniTask<LevelToPlay> GetPuzzleByLevel(int levelId)
        {
            LevelRecord record = _catalog.Levels.FirstOrDefault(level => level.Id == levelId);
            if (record.Id == 0)
            {
                throw new InvalidOperationException("Уровень не найден.");
            }

            return UniTask.FromResult(new LevelToPlay(record.Id, ToDefinition(record)));
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
