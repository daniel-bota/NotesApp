using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

using NotesApp.Data;
using NotesApp.Data.Authentication;
using NotesApp.Models.Authentication;

namespace NotesApp.Core.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly NotesAppDbContext _dbContext;
        private readonly IConfigurationRoot _appConfiguration;

        public string AuthenticatedUsername { get; private set; }
        public string SessionId { get; private set; }

        public AuthenticationManager(NotesAppDbContext dbContext, IConfigurationRoot configuration)
        {
            _dbContext = dbContext;
            _appConfiguration = configuration;
            AuthenticatedUsername = "";
            SessionId = "";
        }

        public async Task<string> RegisterNewUser(RegistrationViewModel model)
        {
            var passwordSalt = await Task.Run(() => GeneratePasswordSalt());
            var passwordHash = await Task.Run(() => ComputePasswordHash(
                model.Password!,
                passwordSalt,
                GetConfiguredPasswordPepper(),
                GetConfiguredHashingIterations()));

            var userAccount = new UserAccount {
                Id = Guid.NewGuid(),
                Username = model.Username!,
                Email = model.Email!,
                Password = passwordHash,
                PasswordSalt = passwordSalt };

            try
            {
                await _dbContext.UserAccounts.AddAsync(userAccount);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return e.InnerException == null ? e.Message : e.InnerException.Message;
            }

            return string.Empty;
        }

        public async Task<bool> ValidateAuthenticationClaim(LoginViewModel model)
        {
            var user = await _dbContext.UserAccounts.FirstOrDefaultAsync<UserAccount>(user => user.Email == model.Email);
            if (user != null && await ValidatePassword(model.Password!, user))
            {
                AuthenticatedUsername = user.Username!;
                return true;
            }

            return false;
        }

        public async Task<string> CreateAuthenticationSession()
        {
            if (string.IsNullOrEmpty(AuthenticatedUsername))
            {
                return "Unauthorized.";
            }

            var user = await _dbContext.UserAccounts.FirstOrDefaultAsync<UserAccount>(user => user.Username == AuthenticatedUsername);
            if (user == null)
            {
                return "Unauthorized";
            }

            var session = new UserAuthenticationSession()
            {
                Id = Guid.NewGuid(),
                UserAccountId = user.Id,
                UserAccount = user
            };

            try
            {
                await _dbContext.UserAuthenticationSessions.AddAsync(session);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return e.InnerException == null ? e.Message : e.InnerException.Message;
            }

            SessionId = ComputeAuthenticationSessionIdHash(session.Id.ToString());
            return string.Empty;
        }

        public async Task<bool> ValidateAuthenticationCookie(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            var decryptedKey = Encoding.UTF8.GetString(Convert.FromBase64String(sessionId));

            //This should be replaced with a query to find the authentication session, not the user
            var session = await _dbContext.UserAuthenticationSessions.Include(session => session.UserAccount).FirstOrDefaultAsync<UserAuthenticationSession>(session => session.Id == new Guid(decryptedKey));

            if (session == null)
            {
                return false;
            }

            AuthenticatedUsername = session.UserAccount!.Username!;
            return true;
        }

        public async Task<bool> ValidateSessionUsername(string username)
        {
            var user = await _dbContext.UserAuthenticationSessions.Include(session => session.UserAccount).FirstOrDefaultAsync<UserAuthenticationSession>(session => session.UserAccount!.Username == username);
            return false;
        }

        private async Task<bool> ValidatePassword(string password, UserAccount user)
        {
            var result = await Task.Run(() => ComputePasswordHash(
                password,
                user.PasswordSalt!,
                GetConfiguredPasswordPepper(),
                GetConfiguredHashingIterations()));

            return result == user.Password;
        }


        private string ComputeAuthenticationSessionIdHash(string sessionId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(sessionId));
        }

        private string ComputePasswordHash(string password, string salt, string pepper, int iterations)
        {
            string hash = password;
            for (int i = 0; i < iterations; i++)
            {
                using var sha = SHA512.Create();
                var passwordSaltPepper = $"{hash}{salt}{pepper}";
                var byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
                var byteHash = sha.ComputeHash(byteValue);
                hash = Convert.ToBase64String(byteHash);
            }

            return hash;
        }
        private string GeneratePasswordSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var byteSalt = new byte[32];
            rng.GetBytes(byteSalt);
            var salt = Convert.ToBase64String(byteSalt);
            return salt;
        }

        private int GetConfiguredHashingIterations()
        {
            var iterations = _appConfiguration.GetValue<int>("SHA512:Iterations");
            return iterations == default(int) ? 1000 : iterations;
        }

        private string GetConfiguredPasswordPepper()
        {
            return _appConfiguration.GetValue<string>("SHA512:PasswordPepper") ?? "";
        }
    }
}
