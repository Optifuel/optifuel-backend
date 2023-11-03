using ApiCos.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCos.Models.Entities
{
    [Table("GasStationRegistries")]
    public class GasStationRegistry : BaseEntity
    {
        public string? CompanyName { get; set; } = null;
        public string? GasProvider { get; set; } = null;
        public string? GasStationName { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? City { get; set; } = null;
        public string? Province { get; set; } = null;
        public double? Latitude { get; set; } = null;
        public double? Longitude { get; set; } = null;
        public List<GasStationPrice> GasStationPrices { get; set; } = new List<GasStationPrice>();
    }
}
