using System;

namespace DuckDoku.Puzzle
{
    public static class PuzzleGenerator
    {
        private const int MaxSizeFactor = 2;

        private const int AttemptLimit = 2000;

        public static PuzzleDefinition Generate(int size, PuzzleDifficulty difficulty, long seed)
        {
            if (size < PuzzleDefinition.MinSize || size > PuzzleDefinition.MaxSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(size), size,
                    $"Размер поля должен быть от {PuzzleDefinition.MinSize} до {PuzzleDefinition.MaxSize}.");
            }

            PuzzleRandom rng = new PuzzleRandom(seed);
            GenerationProfile profile = GenerationProfile.For(size, difficulty);

            for (int attempt = 0; attempt < AttemptLimit; attempt++)
            {
                if (!LevelGenerator.TryBuild(size, rng, out int[] columns))
                {
                    continue;
                }

                int[] caps = profile.RollCaps(size, rng);

                if (!RegionGrower.TryGrow(size, columns, caps, rng, out int[] regions))
                {
                    continue;
                }

                if (!IsWellShaped(size, regions, caps))
                {
                    continue;
                }

                return new PuzzleDefinition(size, seed, difficulty, regions, ToCells(columns));
            }

            throw new InvalidOperationException(
                $"Не удалось сгенерировать уровень {size}x{size} ({difficulty}) за {AttemptLimit} попыток.");
        }

        private static bool IsWellShaped(int size, int[] regions, int[] caps)
        {
            int[] counts = new int[size];
            for (int i = 0; i < regions.Length; i++)
            {
                counts[regions[i]]++;
            }

            int allowed = size * MaxSizeFactor;

            for (int region = 0; region < size; region++)
            {
                if (caps[region] != RegionGrower.NoCap)
                {
                    continue;
                }

                if (counts[region] > allowed)
                {
                    return false;
                }
            }

            return true;
        }

        private static Cell[] ToCells(int[] columns)
        {
            Cell[] cells = new Cell[columns.Length];
            for (int row = 0; row < columns.Length; row++)
            {
                cells[row] = new Cell(row, columns[row]);
            }

            return cells;
        }
    }
}
