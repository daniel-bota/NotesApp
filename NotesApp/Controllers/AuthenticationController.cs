using Microsoft.AspNetCore.Mvc;
using NotesApp.Models.Authentication;
using NotesApp.Core.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data.Authentication;

namespace NotesApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;


        public AuthenticationController(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login(bool force = false, bool registration = false)
        {
            if (force) { return View(); }

            if (registration)
            {
                if (TempData.TryGetValue("RegistrationSuccessMessage", out object? message))
                {
                    ViewBag.RegistrationSuccessMessage = message;
                }
                return View();
            }

            if (HttpContext.Request.Cookies.TryGetValue("AuthenticationSession", out string? sessionKey))
            {
                if(await _authenticationManager.ValidateAuthenticationCookie(sessionKey)
                    && !string.IsNullOrEmpty(_authenticationManager.AuthenticatedUsername))
                {
                    HttpContext.Session.SetString("Username", _authenticationManager.AuthenticatedUsername);
                    return Redirect("/Home/Index");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }

            if (!await _authenticationManager.ValidateAuthenticationClaim(model))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Wrong email or password.");
                return View(model);
            }

            var errorMessage = await _authenticationManager.CreateAuthenticationSession();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.Clear();
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(3);
            HttpContext.Response.Cookies.Append("AuthenticationSession", _authenticationManager.SessionId, cookieOptions);

            HttpContext.Session.SetString("Username", _authenticationManager.AuthenticatedUsername);

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string errorMessage = await _authenticationManager.RegisterNewUser(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            ModelState.Clear();
            TempData.Add("RegistrationSuccessMessage", "You have registered successfully! Please login.");

            return RedirectToAction("Login", new { force=false, registration=true});
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", new { force = true });
            }

            await Task.Run(() =>
            {
                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(-3);
                HttpContext.Response.Cookies.Append("AuthenticationSession", _authenticationManager.SessionId, cookieOptions);

                HttpContext.Session.Remove("Username");
            });

            return RedirectToAction("Login", new { force = true });
        }
    }
}
