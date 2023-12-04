using ApiCos.Models.Entities;

namespace ApiCos.Services.IRepositories
{
    public interface IMapBox
    {
        public Task<Coordinates> GetCoordinates(string city);
        public Task<Models.Entities.Route> GetPathByTown(string startTown, string endTown);
        public Task<List<GasStationRegistry>> FindGasStation(Vehicle vehicle, string startTown, string endTown);
    }
}
