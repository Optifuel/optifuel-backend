using ApiCos.Data;
using ApiCos.Models.Common;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
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
            int token = generateRandomNumber();

            user.Verification = new Verification()
            {
                Token = token,
                DeadLine = DateTime.UtcNow.AddDays(1),
                UserId= user.Id,
                User = user,
            };
            await dbSet.AddAsync(user);
            
            sendVerificationEmail(user.Email,token );
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
            if(user.PasswordEncrypted.Validated== false)
                throw new Exception("Account is not validated");

            return user;
        }

        public async Task<bool> ValidationUser(string email, int token)
        {
            User user = await dbSet.Where(u => u.Email == email).Include(u => u.Verification).FirstOrDefaultAsync();
            if( user == null )
                throw new Exception("user not found");
            if(user.PasswordEncrypted.Validated == true)
                throw new Exception("user is already validated");

            Console.WriteLine($"{user.Verification.Token} TOKEN");
            if(user.Verification.Token == token)
            {
                user.PasswordEncrypted.Validated = true;
                user.Verification = null;
                _context.SaveChanges();
                return true;
            }else
            {
                return false;
            }

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

        public async Task<User?> EditUser(User user)
        {
            var userTable = dbSet.Where(u => u.Email == user.Email).FirstOrDefault();
            if(userTable == null)
                throw new Exception("user not found");
            userTable.Name = user.Name;
            userTable.Surname = user.Surname;
            userTable.DateBirth = user.DateBirth;
            userTable.DrivingLicense = user.DrivingLicense;

            await _context.SaveChangesAsync();
            return userTable;

        }

        private void sendVerificationEmail(string mail, int token)
        {
            string fromMail = "iscosproject@gmail.com";
            string fromPassword = "stmgapzhamxgbbtq";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(new MailAddress(mail));
            message.Body = "<html><body> " +
                "Grazie per esserti registrato al nostro sito. \n" +
                " Per utilizzare tutte le nostre funzionalità inserisci il seguente codice di verifica nell'apposita sezione del sito: \n " + token +
                "</body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }

        private int generateRandomNumber()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999);
        }
    }

}
