using Api.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Entities
{
    [Table("GasStationPrices")]
    public class GasStationPrice : BaseEntity
    {
        public GasStationRegistry GasStationRegistry { get; set; } = null!;
        public string FuelType { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsSelf { get; set; }
        public string LastUpdate { get; set; }

    }
}
