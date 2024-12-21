using Api.Models.Entities;   

namespace Api.DTOs.MapBox
{
    public class FindGasStationSending
    {
        public List<GasStationSending> Station { get; set; }
        public List<Coordinates> coordinates { get; set; }
    }
}
