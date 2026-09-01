using System;
using System.Collections.Generic;

namespace DuckDoku.Puzzle
{
    public sealed class PuzzleDefinition
    {
        public const int MinSize = 6;
        public const int MaxSize = 10;

        public int Size { get; }
        public long Seed { get; }
        public PuzzleDifficulty Difficulty { get; }
        
        public IReadOnlyList<Cell> Solution => _solution;

        private readonly int[] _regions;
        private readonly Cell[] _solution;

        public PuzzleDefinition(int size, long seed, PuzzleDifficulty difficulty,
            int[] regions, Cell[] solution)
        {
            if (size < MinSize || size > MaxSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(size), size, 
                    $"Размер поля должен быть от {MinSize} до {MaxSize}.");
            }

            if (regions == null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            if (solution == null)
            {
                throw new ArgumentNullException(nameof(solution));
            }
            
            int cellCount = size * size;
            if (regions.Length != cellCount)
            {
                throw new ArgumentException(
                    $"Карта областей должна содержать {cellCount} элементов, получено {regions.Length}.",
                    nameof(regions));
            }
            
            bool[] regionUsed = new bool[size];
            for (int index = 0; index < cellCount; index++)
            {
                int region = regions[index];
                if (region < 0 || region >= size)
                {
                    throw new ArgumentException(
                        $"Клетка ({index / size}, {index % size}) ссылается на область {region}, " +
                        $"допустимы значения 0..{size - 1}.",
                        nameof(regions));
                }

                regionUsed[region] = true;
            }
            
            for (int region = 0; region < size; region++)
            {
                if (!regionUsed[region])
                {
                    throw new ArgumentException($"Область {region} не содержит ни одной клетки.", nameof(regions));
                }
            }

            if (solution.Length != size)
            {
                throw new ArgumentException(
                    $"Решение должно содержать {size} клеток, получено {solution.Length}.",
                    nameof(solution));
            }
            
            for (int index = 0; index < solution.Length; index++)
            {
                Cell cell = solution[index];
                if (cell.Row < 0 || cell.Row >= size || cell.Column < 0 || cell.Column >= size)
                {
                    throw new ArgumentException(
                        $"Клетка решения {cell} выходит за пределы поля {size}x{size}.",
                        nameof(solution));
                }
            }
            
            Size = size;
            Seed = seed;
            Difficulty = difficulty;

            _regions = (int[])regions.Clone();
            _solution = (Cell[])solution.Clone();
        }

        public int RegionAt(int row, int column)
        {
            if (row < 0 || row >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (column < 0 || column >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }
            
            return _regions[row * Size + column];
        }

        public int RegionAt(Cell cell)
        {
            return RegionAt(cell.Row, cell.Column);
        }
    }
}