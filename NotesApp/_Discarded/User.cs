using NotesApp.Data.Authentication;

namespace NotesApp._Discarded
{
    public class User(UserAccount userAccount) : IUser
    {
        private readonly UserAccount _userAccount = userAccount;

        public override int GetHashCode()
        {
            return _userAccount.Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(User a, User b)
        {
            return a._userAccount == b._userAccount;
        }

        public static bool operator !=(User a, User b)
        {
            if (a._userAccount == null || b._userAccount == null)
            {
                return false;
            }

            return a._userAccount != b._userAccount;
        }
    }
}
