using System;

namespace DuckDoku.Puzzle
{ 
    // Источники:
    //   xorshift128 — G. Marsaglia, «Xorshift RNGs», Journal of Statistical
    //   Software 8(14), 2003. https://www.jstatsoft.org/article/view/v008i14
    //
    //   splitmix64 — S. Vigna, 2015, эталонная реализация в общественном
    //   достоянии: https://xorshift.di.unimi.it/splitmix64.c
    //   Константы 0x9E3779B97F4A7C15, 0xBF58476D1CE4E5B9, 0x94D049BB133111EB
    //   и сдвиги 30/27/31 взяты оттуда без изменений.
    //
    //   Устранение смещения в NextInt — приём с отбрасыванием хвоста
    //   (rejection sampling), описан у D. Lemire, «Fast Random Integer
    //   Generation in an Interval», 2019.
    //
    public sealed class PuzzleRandom
    {
        private uint _x;
        private uint _y;
        private uint _z;
        private uint _w;

        public PuzzleRandom(long seed)
        {
            ulong state = unchecked((ulong)seed);

            ulong first = NextSplitMix64(ref state);
            ulong second = NextSplitMix64(ref state);

            _x = (uint)first;
            _y = (uint)(first >> 32);
            _z = (uint)second;
            _w = (uint)(second >> 32);
            
            if ((_x | _y | _z | _w) == 0)
            {
                _w = 0x9E3779B9;
            }
        }
        
        public uint NextUInt()
        {
            unchecked
            {
                uint t = _x;
                t ^= t << 11;
                t ^= t >> 8;

                _x = _y;
                _y = _z;
                _z = _w;

                _w ^= _w >> 19;
                _w ^= t;

                return _w;
            }
        }
        
        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(exclusiveMax), exclusiveMax, "Upper bound must be positive.");
            }

            uint bound = (uint)exclusiveMax;
            uint threshold;

            unchecked
            {
                threshold = (0u - bound) % bound;
            }

            while (true)
            {
                uint value = NextUInt();

                if (value >= threshold)
                {
                    return (int)(value % bound);
                }
            }
        }

        public void Shuffle<T>(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            for (int i = items.Length - 1; i > 0; i--)
            {
                int j = NextInt(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }
        
        private static ulong NextSplitMix64(ref ulong state)
        {
            unchecked
            {
                state += 0x9E3779B97F4A7C15UL;

                ulong z = state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;

                return z ^ (z >> 31);
            }
        }
    }
}