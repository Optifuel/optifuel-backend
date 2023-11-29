namespace ApiCos.Models.Entities
{
    public class GeoCodingResponse
    {
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public float[] center { get; set; }
    }
}
