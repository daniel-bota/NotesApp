using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.Authentication
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email must not be empty.")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password must not be empty.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be between 8 and 40 characters long.")]
        [MaxLength(40, ErrorMessage = "Password must be between 8 and 40 characters long.")]
        public string? Password { get; set; }
    }
}
