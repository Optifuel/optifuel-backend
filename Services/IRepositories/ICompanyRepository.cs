using ApiCos.Models.Entities;

namespace ApiCos.Services.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        public Task<Company?> GetCompanyByBusinessName(string businessName);
        public Task<Company?> GetCompanyByVatNumber(string vatNumber);
        public Task<List<Vehicle>?> GetVehiclesByBusinessName(string businessName); 
        public Task<List<User>?> GetUsersByBusinessName(string businessName);
    }
}
