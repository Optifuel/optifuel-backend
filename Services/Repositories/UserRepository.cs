using ApiCos.Data;
using ApiCos.Models.Common;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using ApiCos.ExceptionApi.User;
using ApiCos.ExceptionApi.Company;

namespace ApiCos.Services.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<User> Add(User user, string businessName , string password)
        {
            Company? company = await _context.Company.Where(c => c.BusinessName == businessName).FirstOrDefaultAsync();

            if(company == null)
                throw new CompanyNotFoundException();

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

        private async Task<User?> GetByEmail(string email)
        {
            return await dbSet.Where(u => u.Email == email).Include(u => u.Verification).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new EmailEmptyException();

            if(string.IsNullOrWhiteSpace(password))
                throw new PasswordEmptyException();

            User? user = await GetByEmail(email);

            if(user == null)
                throw new UserNotFoundException();

            if(!ValidatePassword(password, user.PasswordEncrypted.PasswordHash, user.PasswordEncrypted.PasswordSalt))
                throw new WrongPasswordException();

            if(user.PasswordEncrypted.Validated== false)
                throw new UserNotValidatedException();

            return user;
        }

        public async Task ValidationUser(string email, int token)
        {
            User? user = await GetByEmail(email);

            if( user == null )
                throw new UserNotFoundException();

            if(user.PasswordEncrypted.Validated == true)
                throw new UserAlreadyValidatedException();

            if(user.Verification.DeadLine < DateTime.UtcNow)
            {
                user.Verification = null;
                _context.SaveChanges();
                throw new TokenExpiredException();
            }


            if(user.Verification.Token == token)
            {
                user.PasswordEncrypted.Validated = true;
                user.Verification = null;
                _context.SaveChanges();
            }else
            {
                throw new WrongValidationTokenException();
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
            var userTable = await GetByEmail(user.Email);

            if(userTable == null)
                throw new UserNotFoundException();

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
