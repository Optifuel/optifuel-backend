using ApiCos.Data;
using ApiCos.Models.Common;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ApiCos.Services.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<User> Add(User user, string businessName , string password)
        {
            Company company = await _context.Company.Where(c => c.BusinessName == businessName).FirstOrDefaultAsync();
            if(company == null)
                throw new Exception("company not found");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordEncrypted = new Password
            {
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                Validated = false
            };
            user.Company = company;
            await dbSet.AddAsync(user);
            return user;
        }
        

        public async Task<User?> GetByEmail(string email)
        {
            return await dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new Exception("null or empty email or password");
            User user = await dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
            if(user == null)
                throw new Exception("user not found");
            if(!ValidatePassword(password, user.PasswordEncrypted.PasswordHash, user.PasswordEncrypted.PasswordSalt))
                throw new Exception("password is not correct");
            return user;
        }

        private bool ValidatePassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHashByte, out byte[] passwordSaltByte)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSaltByte = hmac.Key;
                passwordHashByte = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }

}
