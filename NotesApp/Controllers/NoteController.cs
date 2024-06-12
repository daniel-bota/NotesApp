using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Data.Note;
using NotesApp.Models.Note;

namespace NotesApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly NotesAppDbContext _dbContext;


        public NoteController(NotesAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> List()
        {
            List<UserNote> notes = await _dbContext.UserNotes.Include(note => note.UserAccount).Where(note => note.UserAccount!.Username == HttpContext.Session.GetString("Username")).OrderByDescending(note => note.LastModified).ToListAsync();
            return View(notes);
        }

        [HttpGet]
        public IActionResult AddNote()
        {
            return View();
        }

		[HttpGet]
		public async Task<IActionResult> EditNote(Guid id)
		{
            var note = await _dbContext.UserNotes.FirstOrDefaultAsync<UserNote>(note => note.Id == id);
            if (note == null)
            {
                return RedirectToAction("List");
            }

            var editNoteModel = new EditNoteViewModel()
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content
            };
            return View(editNoteModel);
		}

        [HttpPost]
        public async Task<IActionResult> EditNote(EditNoteViewModel model)
        {
            model.LastModified = DateTime.Now.ToUniversalTime();

            var note = await _dbContext.UserNotes.FindAsync(model.Id);

            if (note == null)
            {
                return RedirectToAction("List");
            }

            note.Title = model.Title;
            note.Content = model.Content;
            note.LastModified = model.LastModified;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult CreateNote()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(EditNoteViewModel model)
        {
            model.LastModified = DateTime.Now.ToUniversalTime();

            var user = await _dbContext.UserAccounts.FirstOrDefaultAsync(user => user.Username == HttpContext.Session.GetString("Username"));
            if (user == null)
            {
                return RedirectToAction("List");
            }

            var note = new UserNote()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Content = model.Content,
                LastModified = model.LastModified,
                UserAccountId = user.Id
            };

            await _dbContext.UserNotes.AddAsync(note);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNote(EditNoteViewModel model)
        {
            var note = await _dbContext.UserNotes.FindAsync(model.Id);
            if (note == null)
            {
                return RedirectToAction("List");
            }

            _dbContext.UserNotes.Remove(note);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

    }
}
