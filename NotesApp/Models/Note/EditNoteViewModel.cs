using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.Note
{
    public class EditNoteViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Note title cannot be empty.")]
        public string? Title { get; set; }

        public string? Content {  get; set; }

        public DateTime LastModified { get; set; }
    }
}
