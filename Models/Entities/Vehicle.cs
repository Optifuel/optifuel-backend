using ApiCos.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCos.Models.Entities
{
    [Table("Vehicles")]
    public class Vehicle : BaseEntity
    {
        [RegularExpression(@"^[A-Z]{2}[0-9]{3}[A-Z]{2}$", ErrorMessage = "Invalid Format")]
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
        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;
    }

}
