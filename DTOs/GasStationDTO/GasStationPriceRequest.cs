using CsvHelper.Configuration.Attributes;

namespace ApiCos.DTOs.GasStationDTO
{
    public class GasStationPriceRequest
    {
        [Index(0)]
        public int idImpianto { get; set; }
        [Index(1)]
        public string? descCarburante { get; set; } = null;
        [Index(2)]
        public float? prezzo { get; set; } = null;
        [Index(3)]
        public bool? isSelf { get; set; } = null;
        [Index(4)]
        public string? dtComu { get; set; } = null;
    }
}
