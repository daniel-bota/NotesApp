using Microsoft.EntityFrameworkCore;

using NotesApp.Data;
using NotesApp.Core.Authentication;

namespace NotesApp
{
	public class Setup
	{
		public static WebApplication Initialize(ref string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<NotesAppDbContext>(
				options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerLocalDB")));

			builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(5));
			builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration);
			builder.Services.AddScoped<IAuthenticationManager, Core.Authentication.AuthenticationManager>();

			var app = builder.Build();
			//app.UsePathBase("/Authentication/Login");

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseSession();
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			//app.MapControllerRoute(
			//	name: "default",
			//	pattern: "{controller=Authentication}/{action=Login}/{force}/{registration}",
			//	defaults: new { controller="Authentication", action="Login", force=false, registration=false});

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}",
				defaults: new { controller = "Home", action = "Index" });

			return app;
		}
	}
}
