using System;

namespace SiloModelingTaskClient
{
    public static class ModelingUnitConverter
    {
        private const double FeetPerMeter = 3.280839895013123;

        public static double MetersToFeet(double value)
        {
            return value * FeetPerMeter;
        }

        public static double FeetToMeters(double value)
        {
            return value / FeetPerMeter;
        }

        public static double DegreesToRadians(double value)
        {
            return value * Math.PI / 180.0;
        }
    }
}
