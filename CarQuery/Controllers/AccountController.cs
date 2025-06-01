using System.Data.Common;
using System.Linq.Expressions;
using CarQuery.Areas.SuperAdmin.Controllers;
using CarQuery.Services;
using CarQuery.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace CarQuery.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserManagementController> _logger;
        private readonly IPasswordUpdateService _passwordUpdateService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinMagager, ILogger<UserManagementController> logger, IPasswordUpdateService passwordUpdateService)
        {
            _userManager = userManager;
            _signInManager = signinMagager;
            _logger = logger;
            _passwordUpdateService = passwordUpdateService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel registerVm)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = registerVm.UserName,
                    Email = registerVm.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                IdentityResult result = _userManager.CreateAsync(user, registerVm.Password).Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                    return RedirectToAction("OperationResultView", "Admin", new { area = "Admin", succeeded = true, message = "A nova conta de Administrador foi criada com sucesso" });
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerVm);
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
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                EditUserViewModel userVm = new EditUserViewModel(user);
                return View(userVm);
            }
            return View(RedirectToAction("OperationResultView", "Admin", new { area = "Admin", succeeded = false, message = "Erro inesperado. Por favor tente novamente" }));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ChangePassword(EditUserViewModel userVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(userVm.UserId);

                    if (user != null)
                    {
                        var (result, errors) = await _passwordUpdateService.PasswordUpdateAsync(user, userVm.NewPassword);

                        if (!result)
                        {
                            if(errors.Count == 1 && errors[0].Equals("Internal error"))
                            {
                                return RedirectToAction("OperationResultView", "Admin", new
                                {
                                    area = "Admin",
                                    succeeded = false,
                                    message = "Erro inesperado. Não foi possível atualizar a senha. Por favor tente novamente"
                                });
                            }
                            else
                            {
                                foreach(var error in errors)
                                {
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                return View(userVm);
                            }
                        }

                        return RedirectToAction("OperationResultView", "Admin", new
                        {
                            area = "Admin",
                            succeeded = true,
                            message = "A senha foi alterada com sucesso!"
                        });
                    }
                }
                return View(userVm);
            }
            catch (Exception ex)
            {
                _logger.LogError("Account (ChangePassword): {ExceptionType} Erro inesperado ao atualizar senha do usuário {UserId}", ex.GetType().Name, userVm.UserId);
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível atualizar a senha. Por favor tente novamente"
                });
            }
        }
    }
}
