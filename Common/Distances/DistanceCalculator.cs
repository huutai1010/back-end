using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Distances
{
    public class DistanceCalculator
    {
        private static double earthRadius = 6371e3; // metres

        public static double calculate(GeoPoint from, GeoPoint to)
        {
            double lat1 = toRadian(from.Latitude); // lon and lat are in radians
            double lat2 = toRadian(to.Latitude);
            double lon1 = toRadian(from.Longitude);
            double lon2 = toRadian(to.Longitude);

            double havC = Math.Sin((lat2 - lat1) / 2) * Math.Sin((lat2 - lat1) / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin((lon2 - lon1) / 2) * Math.Sin((lon2 - lon1) / 2);

            return 2 * earthRadius * Math.Asin(Math.Sqrt(havC));
        }

        private static double toRadian(double value)
        {
            return value * Math.PI/ 180;
        }
    }
}
