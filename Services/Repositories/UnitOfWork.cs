using Api.Services.IRepositories;
using Api.Data;

namespace Api.Services.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<IUnitOfWork> _logger;

        public IUserRepository User { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IVehicleRepository Vehicle { get; private set; }

        public UnitOfWork(ApiDbContext context, ILogger<IUnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            User = new UserRepository(_context, _logger);
            Company = new CompanyRepository(_context, _logger);
            Vehicle = new VehicleRepository(_context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
