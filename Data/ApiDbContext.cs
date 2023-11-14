using ApiCos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiCos.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<GasStationRegistry> GasStationRegistry { get; set; } = null!;
        public DbSet<Company> Company { get; set; } = null!;
        public DbSet<Vehicle> Vehicle { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<GasStationPrice> GasStationPrice { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder .Entity<GasStationPrice>().Property(e => e.FuelType).HasConversion<string>();

            modelBuilder.Entity<Company>().OwnsOne(e => e.Address);

            modelBuilder.Entity<User>().OwnsOne(e => e.DrivingLicense);
            modelBuilder.Entity<User>().OwnsOne(e => e.PasswordEncrypted);

            modelBuilder.Entity<GasStationPrice>()
                  .HasKey(e => new { e.Id, e.FuelType, e.IsSelf });

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(u => u.BusinessName).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(u=> u.VatNumber).IsUnique();
            modelBuilder.Entity<Vehicle>().HasIndex(u => u.LicensePlate).IsUnique();

            modelBuilder.Entity<Company>().HasMany(e => e.Vehicles).WithOne(e => e.Company).HasForeignKey(e => e.CompanyId).IsRequired(true);
            modelBuilder.Entity<Company>().HasMany(e => e.Users).WithOne(e => e.Company).HasForeignKey(e => e.CompanyId).IsRequired(true);
        }
    }
}
