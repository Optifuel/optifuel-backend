using ApiCos.Models.Entities;   

namespace ApiCos.DTOs.MapBox
{
    public class GasStationSending
    {
        public string name { get; set; }
        public Coordinates coordinates { get; set; }
        public string address { get; set; }
        public string type { get; set; }
        public double price { get; set; }
    }
}
