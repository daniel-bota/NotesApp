using NotesApp.Data.Authentication;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.Data.Note
{
    public class UserNote
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime LastModified { get; set; }

        [ForeignKey("UserAccount")]
        public Guid UserAccountId { get; set; }

        public virtual UserAccount? UserAccount { get; set; }
    }
}
