using CsvHelper.Configuration.Attributes;

namespace Api.DTOs.GasStationDTO
{
    public class GasStationRegistryRequest
    {
        [Index(0)]
        public int idImpianto { get; set; }
        [Index(1)]
        public string? Gestore { get; set; } = null;
        [Index(2)]
        public string? Bandiera { get; set; } = null;
        [Index(3)]
        public string? TipoImpianto { get; set; } = null;
        [Index(4)]
        public string? NomeImpianto { get; set; } = null;
        [Index(5)]
        public string? Indirizzo { get; set; } = null;
        [Index(6)]
        public string? Comune { get; set; } = null;
        [Index(7)]
        public string? Provincia { get; set; } = null;
        [Index(8)]
        public string? Latitudine { get; set; } = null;
        [Index(9)]
        public string? Longitudine { get; set; } = null;
    }
}
