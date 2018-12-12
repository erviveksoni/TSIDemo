using System;

namespace Simulator
{
    public class Utility
    {
        static Random rand = new Random();

        public static int GetFloorIncremental(int current, int minFloor, int maxFloor)
        {
            var newValue = rand.Next(minFloor, maxFloor);
            if (newValue == current)
            {
                current = newValue + 1;
            }
            else
            {
                current = newValue;
            }
            
            if (current > maxFloor)
            {
                return current - 3;
            }

            return current;
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

        public static double DecrementValue(double current, double min)
        {
            if (current <= min)
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
