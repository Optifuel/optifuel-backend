using ApiCos.Models.Entities;

namespace ApiCos.DTOs.VehicleDTO
{
    public class VehicleSending
    {
        public string LicensePlate { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string FuelType { get; set; } = null!;
        public int EngineDisplacement { get; set; }
        public double Km { get; set; }
        public int LitersTank { get; set; }
        public float Weight { get; set; }
        public float MaxLoad { get; set; }
        public float UrbanConsumption { get; set; }
        public float ExtraUrbanConsumption { get; set; }
    }
}
