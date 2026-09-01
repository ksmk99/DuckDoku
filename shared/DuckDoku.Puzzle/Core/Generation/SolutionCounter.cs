using System;

namespace DuckDoku.Puzzle
{
    public static class SolutionCounter
    {
        public const int Unassigned = -1;

        private const int NoPreviousColumn = -2;

        public static int Count(int size, int[] regions, int limit)
        {
            if (regions == null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            if (limit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Предел должен быть положительным.");
            }

            int found = 0;
            Search(size, regions, 0, 0, 0, NoPreviousColumn, limit, ref found);

            return found;
        }

        private static void Search(
            int size,
            int[] regions,
            int row,
            int columnsMask,
            int regionsMask,
            int previousColumn,
            int limit,
            ref int found)
        {
            if (row == size)
            {
                found++;
                return;
            }

            int rowOffset = row * size;

            for (int column = 0; column < size; column++)
            {
                if ((columnsMask & (1 << column)) != 0)
                {
                    continue;
                }

                int region = regions[rowOffset + column];
                if (region == Unassigned)
                {
                    continue;
                }

                if ((regionsMask & (1 << region)) != 0)
                {
                    continue;
                }

                int distance = column - previousColumn;
                if (distance >= -1 && distance <= 1)
                {
                    continue;
                }

                Search(
                    size,
                    regions,
                    row + 1,
                    columnsMask | (1 << column),
                    regionsMask | (1 << region),
                    column,
                    limit,
                    ref found);

                if (found >= limit)
                {
                    return;
                }
            }
        }
    }
}
