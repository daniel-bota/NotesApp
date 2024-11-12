using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.Authentication
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Username must not be empty.")]
        [MaxLength(100, ErrorMessage = "Username most not be longer than 100 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email must not be empty.")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(200, ErrorMessage = "Email address must not be longer than 200 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password must not be empty.")]
        [DataType(DataType.Password)]
        [Length(8, 40, ErrorMessage = "Password must be between 8 and 40 characters long.")]
        public string? Password { get; set; }
    }
}
