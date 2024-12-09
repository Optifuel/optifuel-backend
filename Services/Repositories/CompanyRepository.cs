using Api.Data;
using Api.ExceptionApi.Company;
using Api.Models.Entities;
using Api.Services.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace Api.Services.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<Company?> Add(Company company)
        {
            if(!await CheckVatValidity(company.VatNumber))
                throw new VatNumberNotValid();

            await dbSet.AddAsync(company);
            return company;

        }   

        public async Task<Company?> GetCompanyByBusinessName(string businessName)
        {
            if(string.IsNullOrEmpty(businessName))
                throw new BussinessnameEmptyException();

            Company? company = await dbSet.Where(c => c.BusinessName == businessName).Include(c => c.Users).Include(c => c.Vehicles).FirstOrDefaultAsync();

            if(company == null)
                throw new CompanyNotFoundException();

            return company;
        }

        public async Task<Company?> GetCompanyByVatNumber(string vatNumber)
        {
            if(string.IsNullOrEmpty(vatNumber))
                throw new VatNumberEmptyException();

            Company? company = await dbSet.Where(c => c.VatNumber == vatNumber).Include(c => c.Users).Include(c => c.Vehicles).FirstOrDefaultAsync();

            if(company == null)
                throw new CompanyNotFoundException();

            return company;
        }

        public async Task<List<User>?> GetUsersByBusinessName(string businessName)
        {
            Company? company = await GetCompanyByBusinessName(businessName);

            return company.Users;
        }

        public async Task<List<Vehicle>?> GetVehiclesByBusinessName(string businessName)
        {
            Company? company = await GetCompanyByBusinessName(businessName);

            return company.Vehicles;
        }

        private async Task<bool> CheckVatValidity(string vatNumber)
        {
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://ec.europa.eu/taxation_customs/vies/rest-api/ms/IT/vat/" + vatNumber;

                    HttpResponseMessage response = await client.GetAsync(url);

                    if(response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);


                        if(result != null && result.isValid != null)
                        {
                            bool isValid = (bool)result.isValid;
                            return isValid;
                        }
                        else
                        {
                            Console.WriteLine("The 'isValid' field is not present in the JSON response or is null.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error in the request: {response.StatusCode}");
                    }
                } catch(Exception ex)
                {
                    Console.WriteLine($"An error has occurred: {ex.Message}");
                }
                return false;
            }
        }

    }
}
