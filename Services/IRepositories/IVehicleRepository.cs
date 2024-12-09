using Api.Models.Entities;

namespace Api.Services.IRepositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        public Task<Vehicle?> Add(Vehicle vehicle, Company company, User user);
        public Task Delete(string licensePlate);
        public Task<Vehicle?> GetByLicensePlate(string licensePlate);
        public Task<Vehicle?> EditVehicle (Vehicle vehicle, string company);

        public Task<List<Vehicle?>?> SearchByBrand(string businessName, string brand);
        public Task<List<Vehicle?>?> SearchByModel(string businessNam, string model);


    }
}
