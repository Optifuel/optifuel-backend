using ApiCos.Data;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace ApiCos.Services.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<Company?> Add(Company company)
        {


            if (!await CheckVatValidity(company.VatNumber))
                throw new Exception("vat number not valid");

            try
            {
                await dbSet.AddAsync(company);
                return company;
            } catch(Exception ex)
            {
                _logger.LogError($"Error in {nameof(Add)}: " + ex.Message);
                return null;
            }
        }   

        public async Task<Company?> GetCompanyByBusinessName(string businessName)
        {
            return await dbSet.Where(c => c.BusinessName == businessName).FirstOrDefaultAsync();
        }

        public async Task<Company?> GetCompanyByVatNumber(string vatNumber)
        {
            return await dbSet.Where(c => c.VatNumber == vatNumber).FirstOrDefaultAsync();
        }

        public async Task<List<User>?> GetUsersByBusinessName(string businessName)
        {
            Company? company = await _context.Company
                                     .Include(c => c.Users)
                                     .FirstOrDefaultAsync(c => c.BusinessName == businessName);
            if(company == null)
                throw new Exception("company not found");
            Console.WriteLine(company.Users.Count);
            return company.Users;
        }

        public async Task<List<Vehicle>?> GetVehiclesByBusinessName(string businessName)
        {
            Company? company = await _context.Company
                                     .Include(c => c.Vehicles)
                                     .FirstOrDefaultAsync(c => c.BusinessName == businessName);
            if (company == null)
                throw new Exception("company not found");

            return company.Vehicles;
        }

        // Assicurati di avere "using System.Threading.Tasks;" all'inizio del tuo file C#
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
