using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usrMngr;
        private readonly SignInManager<AppUser> _sgnInMngr;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _usrMngr = userManager;
            _sgnInMngr = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            var model = new LoginModel()
            {
                Username = string.Empty,
                Password = string.Empty,
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usrMngr.FindByNameAsync(model.Username);
                if (user != null)
                {
                    await _sgnInMngr.SignOutAsync();
                    var result = await _sgnInMngr.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(model.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Nutzername oder Passwort ungültig!");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _sgnInMngr.SignOutAsync();
            return Redirect(returnUrl ?? "/");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View("AccessDenied", returnUrl);
        }
    }
}
