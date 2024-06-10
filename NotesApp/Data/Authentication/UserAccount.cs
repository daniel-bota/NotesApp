using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using NotesApp.Data.Note;

namespace NotesApp.Data.Authentication
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class UserAccount
	{
		public Guid Id { get; set; }

        [Required(ErrorMessage = "Username must not be empty.")]
        [MaxLength(100, ErrorMessage = "Username most not be longer than 100 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email must not be empty.")]
        [MaxLength(200, ErrorMessage = "Email address must not be longer than 200 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password must not be empty.")]
        [Length(8, 40, ErrorMessage = "Password must be between 8 and 40 characters long.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "User-specific encryption salt was not provided.")]
        public string? PasswordSalt { get; set; }

        public virtual ICollection<UserAuthenticationSession>? UserAuthenticationSessions { get; }
        public virtual ICollection<UserNote>? Notes { get; }
    }
}
