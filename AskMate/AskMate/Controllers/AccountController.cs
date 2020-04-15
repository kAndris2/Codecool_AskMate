using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AskMate.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using Microsoft.Extensions.Logging;
using AskMate.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AskMate.Controllers
{
    public class AccountController : Controller
    {
        IDAO_Impl Idao = IDAO_Impl.Instance;

        [HttpGet]
        public ViewResult Register()
        {
            return View();
        }
        public class RegisterViewModel
        {
            [Required, MaxLength(256)]
            public string Username { get; set; }
            [Required, MaxLength(256)]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password), Compare(nameof(Password))]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Idao.Register(model.Username, model.Email, model.Password);
                return View("RegSuccess");
            }
            else { return View(); }
        }
        //logintest--------------------------------------------------------------------------------------------------------------
        private readonly ILogger<AccountController> _logger;

        private readonly IUserService _userService;

        public AccountController(ILogger<AccountController> logger, IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync([FromForm] string email, [FromForm] string password)
        {
            UserModel user = _userService.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
                
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email) };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Profile");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
