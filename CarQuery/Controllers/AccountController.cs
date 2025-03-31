using System.Data.Common;
using System.Linq.Expressions;
using CarQuery.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinMagager)
        {
            _userManager = userManager;
            _signInManager = signinMagager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVm, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(loginVm.Email);

                    if (user != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, false, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }
                }
                TempData["ErrorMessage"] = "Email ou senha incorretos";
                return View(loginVm);
            }
            catch(Exception ex)
            {
                return View();
            }
        }
    }
}
