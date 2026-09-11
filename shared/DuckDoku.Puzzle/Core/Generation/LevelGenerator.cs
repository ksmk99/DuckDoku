using System;

namespace DuckDoku.Puzzle
{
    public class LevelGenerator
    {
        private const int NoPreviousColumn = -2;

        public static bool TryBuild(int size, PuzzleRandom rng, out int[] columns)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Board size must be positive.");
            }

            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }
            
            int[][] columnOrder = new int[size][];
            for (int row = 0; row < size; row++)
            {
                columnOrder[row] = CreateShuffledOrder(size, rng);
            }

            int[] buffer = new int[size];
            if (Search(0, 0, NoPreviousColumn, buffer, columnOrder))
            {
                columns = buffer;
                return true;
            }
            
            columns = Array.Empty<int>();
            return false;
        }

        private static bool Search(
            int row,
            int columnsMask,
            int previousColumn,
            int[] buffer,
            int[][] columnOrder)
        {
            int size = buffer.Length;
            if (row == size)
            {
                return true;
            }

            int[] candidates = columnOrder[row];
            for (int i = 0; i < size; i++)
            {
                int column = candidates[i];
                if ((columnsMask & (1 << column)) != 0)
                {
                    continue;
                }
                
                int distance = column - previousColumn;
                if (distance >= -1 && distance <= 1)
                {
                    continue;
                }

                buffer[row] = column;
                if (Search(row + 1, columnsMask | (1 << column), column, buffer, columnOrder))
                {
                    return true;
                }
            }

            return false;
        }

        private static int[] CreateShuffledOrder(int size, PuzzleRandom rng)
        {
            int[] order = new int[size];
            for (int i = 0; i < size; i++)
            {
                order[i] = i;
            }

            rng.Shuffle(order);

            return order;
        }
    }
}