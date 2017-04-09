using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public static class GlobalRNG
    {
        private static int seed;

        private static Random rng;

        public static void Initialize(int overrideSeed = 0)
        {
            seed = System.DateTime.Now.Millisecond * System.DateTime.Now.Second;

            if (overrideSeed > 0)
                seed = overrideSeed;

            rng = new Random(seed);
            Record.WriteLine("Global RNG engine initialized. Seed is {0}.\n", seed);
        }

        public static int Next(int min, int max)
        {
            return rng.Next(min, max + 1);
        }

        private static List<int> cache = new List<int>();

        // This function rerolls rng.Next() as many times as it needs to to create different values.
        // This is used for setting characters at the beginning of the game.
        public static int[] DistinctValues(int min, int max, int num)
        {

            cache.Clear(); // Empty cache for repopulation
            int[] i = new int[num];
            int candidate;
            
            while (cache.Count < num) // Keep adding distinct values to cache
            {
                candidate = rng.Next(min, max + 1);
                if (!cache.Contains(candidate))
                    cache.Add(candidate);
            }

            for (int a = 0; a < num; a++)
            {
                i[a] = cache[a];
            }

            return i;
        }

        public static void SeedOverride(int s)
        {
            seed = s;
            rng = new Random(seed);
            Record.WriteLine("Seed overridden! New seed is {0}.\n", s);
        }
    }
}
