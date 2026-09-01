using System;

namespace DuckDoku.Puzzle
{
    public static class RegionGrower
    {
        public const int NoCap = int.MaxValue;

        private const int Unassigned = SolutionCounter.Unassigned;

        public static bool TryGrow(
            int size,
            int[] solutionColumns,
            int[] caps,
            PuzzleRandom rng,
            out int[] regions)
        {
            if (solutionColumns == null)
            {
                throw new ArgumentNullException(nameof(solutionColumns));
            }

            if (caps == null)
            {
                throw new ArgumentNullException(nameof(caps));
            }

            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }

            if (solutionColumns.Length != size)
            {
                throw new ArgumentException(
                    $"Расстановка должна содержать {size} столбцов, получено {solutionColumns.Length}.",
                    nameof(solutionColumns));
            }

            if (caps.Length != size)
            {
                throw new ArgumentException(
                    $"Потолков должно быть {size}, получено {caps.Length}.",
                    nameof(caps));
            }

            int cellCount = size * size;

            regions = new int[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                regions[i] = Unassigned;
            }

            int[] sizes = new int[size];
            for (int row = 0; row < size; row++)
            {
                regions[row * size + solutionColumns[row]] = row;
                sizes[row] = 1;
            }

            int target = TargetSize(size, caps);
            int remaining = cellCount - size;

            int[] moveCells = new int[cellCount * 4];
            int[] moveRegions = new int[cellCount * 4];
            int[] moveWeights = new int[cellCount * 4];

            while (remaining > 0)
            {
                int moveCount = CollectMoves(size, regions, sizes, caps, target, moveCells, moveRegions, moveWeights);
                if (moveCount == 0)
                {
                    return false;
                }

                long weightSum = 0;
                for (int i = 0; i < moveCount; i++)
                {
                    weightSum += moveWeights[i];
                }

                bool placed = false;

                while (!placed && moveCount > 0)
                {
                    int pick = Roulette(moveWeights, moveCount, weightSum, rng);

                    int cell = moveCells[pick];
                    int region = moveRegions[pick];

                    int last = moveCount - 1;
                    weightSum -= moveWeights[pick];
                    moveCells[pick] = moveCells[last];
                    moveRegions[pick] = moveRegions[last];
                    moveWeights[pick] = moveWeights[last];
                    moveCount--;

                    regions[cell] = region;

                    if (SolutionCounter.Count(size, regions, 2) == 1)
                    {
                        sizes[region]++;
                        remaining--;
                        placed = true;
                    }
                    else
                    {
                        regions[cell] = Unassigned;
                    }
                }

                if (!placed)
                {
                    return false;
                }
            }

            return true;
        }

        private static int TargetSize(int size, int[] caps)
        {
            int zoneCells = 0;
            int zoneCount = 0;

            for (int region = 0; region < size; region++)
            {
                if (caps[region] == NoCap)
                {
                    continue;
                }

                zoneCells += caps[region];
                zoneCount++;
            }

            if (zoneCount >= size)
            {
                return 1;
            }

            return (size * size - zoneCells) / (size - zoneCount);
        }

        private static int Roulette(int[] weights, int count, long weightSum, PuzzleRandom rng)
        {
            if (weightSum <= 0)
            {
                return rng.NextInt(count);
            }

            long ticket = rng.NextInt((int)Math.Min(weightSum, int.MaxValue));

            for (int i = 0; i < count; i++)
            {
                ticket -= weights[i];
                if (ticket < 0)
                {
                    return i;
                }
            }

            return count - 1;
        }

        private static int CollectMoves(
            int size,
            int[] regions,
            int[] sizes,
            int[] caps,
            int target,
            int[] cells,
            int[] targets,
            int[] weights)
        {
            int count = 0;

            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    int index = row * size + column;
                    if (regions[index] != Unassigned)
                    {
                        continue;
                    }

                    Add(size, regions, sizes, caps, target, index, row - 1, column, cells, targets, weights, ref count);
                    Add(size, regions, sizes, caps, target, index, row + 1, column, cells, targets, weights, ref count);
                    Add(size, regions, sizes, caps, target, index, row, column - 1, cells, targets, weights, ref count);
                    Add(size, regions, sizes, caps, target, index, row, column + 1, cells, targets, weights, ref count);
                }
            }

            return count;
        }

        private static void Add(
            int size,
            int[] regions,
            int[] sizes,
            int[] caps,
            int target,
            int index,
            int row,
            int column,
            int[] cells,
            int[] targets,
            int[] weights,
            ref int count)
        {
            if (row < 0 || row >= size || column < 0 || column >= size)
            {
                return;
            }

            int region = regions[row * size + column];
            if (region == Unassigned)
            {
                return;
            }

            if (sizes[region] >= caps[region])
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (cells[i] == index && targets[i] == region)
                {
                    return;
                }
            }

            cells[count] = index;
            targets[count] = region;

            int slack = target - sizes[region];
            weights[count] = slack > 0 ? slack + 1 : 1;

            count++;
        }
    }
}
