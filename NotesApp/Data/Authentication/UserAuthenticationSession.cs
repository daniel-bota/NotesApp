using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace NotesApp.Data.Authentication
{
    public class UserAuthenticationSession
    {
        public Guid Id { get; set; }

        public DateTime ExpiryDate { get; set; }

        [ForeignKey("UserAccount")]
        public Guid UserAccountId { get; set; }

        public virtual UserAccount? UserAccount { get; set; }
    }
}
