﻿using ApiCos.Data;
using ApiCos.Models.Common;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using ApiCos.ExceptionApi.User;

namespace ApiCos.Services.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApiDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<User> Add(User user, Company company, string password)
        {
            var userFound = await GetByEmail(user.Email);
            if(userFound != null)
                throw new UserAlreadyExistException();

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
            string text = " Per utilizzare tutte le nostre funzionalità inserisci il seguente codice di verifica nell'apposita sezione del sito: \n ";
            sendVerificationEmail(user.Email,token, text );
            return user;
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await dbSet.Where(u => u.Email == email).Include(u => u.Verification).Include(u => u.ChangePassword).Include(u => u.Vehicles).Include(u=> u.Company).FirstOrDefaultAsync();
            return user;
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

        public async Task<List<Vehicle>> GetListVehicleByUser(string email)
        {
            var user = await GetByEmail(email);
            return user.Vehicles;
        }

        private void sendVerificationEmail(string mail, int token, string text)
        {
            string fromMail = "optifueldp@gmail.com";
            string fromPassword = "wwgonwqktgrvlulh";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Conferma Registrazione";
            message.To.Add(new MailAddress(mail));
            message.Body = "<html><body> " +
                "Grazie per esserti registrato al nostro sito. \n" +
               text + token +
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

        public async Task ChangePasswordRequest(string email, string oldPassword, string newPassword)
        {
            User user = GetByEmailAndPassword(email, oldPassword).Result;
            CreatePasswordHash(newPassword, out var passwordHash, out var passwordSalt);

            user.ChangePassword = new ChangePassword()
            {
                Token = generateRandomNumber(),
                NewPasswordSalt = passwordSalt,
                NewPasswordHash = passwordHash,
                DeadLine = DateTime.UtcNow.AddDays(1),
                UserId = user.Id,
                User = user,
            };

            string text = " Per modificare la tua password inserisci il seguente codice nell'apposita sezione del sito: \n ";
            sendVerificationEmail(user.Email, user.ChangePassword.Token, text);
            _context.SaveChanges();


        }

        public async Task ChangePassword(string email, int token)
        {
            User user = await GetByEmail(email);
            if(user.ChangePassword == null)
                throw new ChangePasswordRequestNotFoundException();

            if(user.ChangePassword.Token != token)
                throw new WrongChangePasswordTokenException();

            if(user.ChangePassword.DeadLine < DateTime.UtcNow)
            {
                user.ChangePassword = null;
                _context.SaveChanges();
                throw new TokenExpiredException();
            }

            user.PasswordEncrypted.PasswordHash = user.ChangePassword.NewPasswordHash;
            user.PasswordEncrypted.PasswordSalt = user.ChangePassword.NewPasswordSalt;
            user.ChangePassword = null;
            _context.SaveChanges();
        }
    }

}
