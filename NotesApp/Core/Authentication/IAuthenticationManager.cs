using Microsoft.EntityFrameworkCore;
using NotesApp.Data.Authentication;
using NotesApp.Data;
using NotesApp.Models.Authentication;

namespace NotesApp.Core.Authentication
{
    public interface IAuthenticationManager
    {
        public string AuthenticatedUsername { get; }
        //public string SessionId { get; }
        public UserAuthenticationSession? Session { get; }

        /// <summary>
        ///     Adds a new user to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        ///     string.Empty if successful.Error message if failed.
        /// </returns>
        Task<string> RegisterNewUser(RegistrationViewModel model);

        Task<bool> ValidateAuthenticationClaim(LoginViewModel model);

        /// <summary>
        ///     Adds a new authentication session to the database.
        ///     The session will have a user id that matches the user 
        ///     with the currently set AuthenticatedUsername.
        ///     Stores the session id hash in the SessionId property.
        ///     This will be used to create the login cookie by the controller.
        /// </summary>
        /// <returns>
        ///     string.Empty if successful.Error message if failed.
        /// </returns>
        Task<string> CreateAuthenticationSession(DateTime expiryDate);
        Task<bool> ValidateAuthenticationCookie(string sessionId);
        Task<bool> ValidateSessionUsername(string username);
        Task<string> DeleteExpiredAuthenticationSessions(string username);

        string ComputeAuthenticationSessionIdHash(string sessionId);


    }
}
