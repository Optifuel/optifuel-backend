using ApiCos.Data;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiCos.Services.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public Task<Company?> GetCompanyByBusinessName(string businessName)
        {
            return dbSet.Where(c => c.BusinessName == businessName).FirstOrDefaultAsync();
        }

        public Task<Company?> GetCompanyByVatNumber(string vatNumber)
        {
            return dbSet.Where(c => c.VatNumber == vatNumber).FirstOrDefaultAsync();
        }

        public Task<List<User>?> GetUsersByBusinessName(string businessName)
        {
            Company company=  _context.Company
                                     .Include(c => c.Users)
                                     .FirstOrDefault(c => c.BusinessName == businessName);
            if(company == null)
                throw new Exception("company not found");
            Console.WriteLine(company.Users.Count);
            return Task.FromResult(company.Users); 
        }

        public Task<List<Vehicle>?> GetVehiclesByBusinessName(string businessName)
        {
            throw new NotImplementedException();
        }
    }
}
