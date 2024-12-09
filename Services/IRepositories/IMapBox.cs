using Api.Models.Entities;

namespace Api.Services.IRepositories
{
    public interface IMapBox
    {
        public Task<Coordinates> GetCoordinates(string city);
        public Task<Models.Entities.Route> GetPathByTown(double initLongitude, double initLatitude, double endLongitude, double endLatitude);
        public Task<List<GasStationRegistry>> FindGasStation(Vehicle vehicle, double percentTank, List<Coordinates> listPoints);
    }
}
