using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using System.Security.Cryptography;

namespace NotesApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var bytePepper = new byte[32];
            RandomNumberGenerator.Create().GetBytes(bytePepper);
            Console.WriteLine(Convert.ToBase64String(bytePepper));
            
			var app = Setup.Initialize(ref args);
			app.Run();
		}
	}
}
