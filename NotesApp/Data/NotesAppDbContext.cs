using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using NotesApp.Data.Authentication;
using NotesApp.Data.Note;

namespace NotesApp.Data
{
	public class NotesAppDbContext : DbContext
	{
		public NotesAppDbContext(DbContextOptions<NotesAppDbContext> options) : base(options) { }
		
		
		public DbSet<UserAccount> UserAccounts { get; set; }
		public DbSet<UserAuthenticationSession> UserAuthenticationSessions { get; set; }
		public DbSet<UserNote> UserNotes { get; set; }
	}
}
