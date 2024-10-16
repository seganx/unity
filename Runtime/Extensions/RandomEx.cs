using System;

namespace SeganX
{
    public static class RandomEx
    {
        /// <summary>
        /// Returns a random point inside or on a circle with radius 1.0
        /// </summary>
        /// <returns>position</returns>
        public static UnityEngine.Vector3 InsideUnitCircle(this Random random)
        {
            double angle = 2 * Math.PI * random.NextDouble();
            double radius = Math.Sqrt(random.NextDouble());

            double x = radius * Math.Cos(angle);
            double y = radius * Math.Sin(angle);

            return new UnityEngine.Vector3((float)x, (float)y);
        }

        /// <summary>
        /// Returns a random float within [minInclusive..maxInclusive] 
        /// </summary>
        /// <param name="min">Inclusive</param>
        /// <param name="max">Inclusive</param>
        /// <returns>randomValue</returns>
        public static float Next(this Random random, float min, float max)
        {
            // TODO: witch value for range01 is better?
            var range01 = 100000;
            // range01+1 used because the upper bound of random is exclusive
            var randomValue01 = random.Next(0, range01 + 1) / (float)range01; // random value in [0,1] range.
            return min + randomValue01 * (max - min); // apply min and max
        }
    }
}