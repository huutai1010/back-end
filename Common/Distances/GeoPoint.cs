
namespace Common.Distances
{
    public class GeoPoint
    {
        public double Latitude;
        public double Longitude;
        public GeoPoint()
        {
            
        }
        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
