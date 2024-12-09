namespace Api.Models.Entities
{
    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static explicit operator Coordinates(List<double> v)
        {
            return new Coordinates
            {
                Latitude = v[1],
                Longitude = v[0]
            };
        }
    }
}
