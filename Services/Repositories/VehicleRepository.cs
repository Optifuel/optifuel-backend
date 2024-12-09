using Api.Data;
using Api.ExceptionApi.Company;
using Api.Models.Entities;
using Api.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using Api.ExceptionApi.Vehicle;
using Api.ExceptionApi.User;

namespace Api.Services.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<Vehicle> Add(Vehicle vehicle, Company company, User user)
        {
            vehicle.Company = company;
            vehicle.CompanyId = vehicle.Company.Id;
            vehicle.User = user;
            vehicle.UserId = vehicle.User.Id;
            await Add(vehicle);
            return vehicle;
        }

        public async Task Delete(string licensePlate)
        {
            Vehicle vehicle = await GetByLicensePlate(licensePlate); 
            await Delete(vehicle.Id);
        }

        public async Task<Vehicle?> EditVehicle(Vehicle vehicle, string company)
        {
            var vehicleToEdit = await dbSet.Where(v => v.LicensePlate == vehicle.LicensePlate).Include(v => v.Company).FirstOrDefaultAsync();
            if (vehicleToEdit == null)
                throw new VehicleNotFoundException();

            var companyDb = await _context.Company.Where(c => c.BusinessName == company).FirstOrDefaultAsync();

            vehicleToEdit.Model = vehicle.Model;
            vehicleToEdit.Brand = vehicle.Brand;
            vehicleToEdit.FuelType = vehicle.FuelType;
            vehicleToEdit.EngineDisplacement = vehicle.EngineDisplacement;
            vehicleToEdit.Km = vehicle.Km;
            vehicleToEdit.LitersTank = vehicle.LitersTank;
            vehicleToEdit.Weight = vehicle.Weight;
            vehicleToEdit.MaxLoad = vehicle.MaxLoad;
            vehicleToEdit.UrbanConsumption = vehicle.UrbanConsumption;
            vehicleToEdit.ExtraUrbanConsumption = vehicle.ExtraUrbanConsumption;
            vehicleToEdit.CompanyId = companyDb.Id;
            vehicleToEdit.Company = companyDb;

            return vehicleToEdit;
    }

        public async Task<Vehicle?> GetByLicensePlate(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new LicensePlateEmptyException();
            var vehicle = await dbSet.Where(v => v.LicensePlate == licensePlate).Include(v => v.Company).Include(v=> v.User).FirstOrDefaultAsync();
            if(vehicle == null)
                throw new VehicleNotFoundException();
            return vehicle;
        }

        public async Task<List<Vehicle?>?> SearchByBrand(string businessName, string brand)
        {
            var list = await dbSet.Where(v => v.Company.BusinessName == businessName && v.Brand == brand).Include(v => v.Company).ToListAsync();
            if(list == null)
                throw new VehicleNotFoundException();
            return list;
        }

        public async Task<List<Vehicle?>?> SearchByModel(string businessNam, string model)
        {
            var list = await dbSet.Where(v => v.Company.BusinessName == businessNam && v.Model == model).Include(v => v.Company).ToListAsync();
            if (list == null)
                throw new VehicleNotFoundException();
            return list;
        }
    }
    
    
}
