using ApiCos.Models.Entities;

namespace ApiCos.Services.IRepositories
{
    public interface IMapBox
    {
        public Task<Coordinates> GetCoordinates(string city);
        public Task<List<List<double>>> GetPathByTown(string startTown, string endTown);
    }
}
