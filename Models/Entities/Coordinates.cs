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

        public override bool Equals(object? obj)
        {
            if (obj is Coordinates other)
            {
                return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
            }
            return false;
        }

        // Sovrascrive GetHashCode per supportare l'uso in dizionari e hash set
        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }
    }
}
