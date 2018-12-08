using System;

namespace Simulator
{
    public class Utility
    {
        static Random rand = new Random();

        public static int GetFloorIncremental(int current, int minFloor, int maxFloor)
        {
            if (current == minFloor)
            {
                return current + 1;
            }
            if (current == maxFloor)
            {
                return current - 5;
            }

            if (rand.Next(minFloor, maxFloor) == current)
            {
                current = current - 1;
            }

            return current + 1;
        }

        public static int GetFloorRandom(int minFloor, int maxFloor)
        {
            return rand.Next(minFloor, maxFloor);
        }

        public static double IncrementValue(double current, double max)
        {
            if (current <= max)
            {
                current = current + rand.NextDouble();
            }
            else
            {
                current = current - rand.NextDouble();
            }

            return current;
        }
    }
}
