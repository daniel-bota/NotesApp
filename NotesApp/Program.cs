using Microsoft.EntityFrameworkCore;
using NotesApp.Data;

namespace NotesApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var app = Setup.Initialize(ref args);
			app.Run();
		}
	}
}
