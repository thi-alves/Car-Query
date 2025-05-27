using System.Data.Common;
using System.Linq.Expressions;
using CarQuery.Areas.SuperAdmin.Controllers;
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

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinMagager, ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _signInManager = signinMagager;
            _logger = logger;
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
                if (registerVm.Password != registerVm.ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "As senhas não coincidem";
                    return View(registerVm);
                }
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
                TempData["ErrorMessage"] = "";
                int minLenght = _userManager.Options.Password.RequiredLength;

                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            TempData["ErrorMessage"] += "O nome de usuário já está sendo usado<br/>";
                            break;
                        case "DuplicateEmail":
                            TempData["ErrorMessage"] += "O email informado já está cadastrado<br/>";
                            break;
                        case "PasswordTooShort":
                            TempData["ErrorMessage"] += "A senha deve ter no mínimo " + minLenght + " caracteres<br/>";
                            break;
                    }
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
                        var passwordValidation = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, userVm.NewPassword);

                        if (!passwordValidation.Succeeded)
                        {
                            foreach (var error in passwordValidation.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(userVm);
                        }

                        //usado para repetir operações caso elas falhem
                        var retryPolicy = Policy
                            .HandleResult<bool>(result => result == false)
                            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

                        var hasPassword = await _userManager.HasPasswordAsync(user);
                        //se o usuário não tiver senha, da erro no RemovePassword
                        if (hasPassword)
                        {
                            IdentityResult removePassIdentityResult = IdentityResult.Failed();
                            bool removeResult = await retryPolicy.ExecuteAsync(async () =>
                            {
                                var result = await _userManager.RemovePasswordAsync(user);
                                removePassIdentityResult = result;
                                return result.Succeeded;
                            });

                            if (!removeResult)
                            {
                                _logger.LogError("Account (ChangePassword): erro ao remover a senha do usuário {UserId}. Erro {Error}", user.Id, string.Join(", ", removePassIdentityResult.Errors.Select(e => e.Description)));
                                return RedirectToAction("OperationResultView", "Admin", new
                                {
                                    area = "Admin",
                                    succeeded = false,
                                    message = "Erro inesperado. Não foi possível atualizar a senha. Por favor tente novamente"
                                });
                            }
                        }
                        IdentityResult addPassIdentityResult = IdentityResult.Failed();
                        bool addPasswordResult = await retryPolicy.ExecuteAsync(async () =>
                        {
                            var result = await _userManager.AddPasswordAsync(user, userVm.NewPassword);
                            addPassIdentityResult = result;
                            return result.Succeeded;
                        });

                        if (!addPasswordResult)
                        {
                            _logger.LogError("Account (ChangePassword): erro ao adicionar senha ao usuário {UserId}. Erro {Error}", user.Id, string.Join(", ", addPassIdentityResult.Errors.Select(e => e.Description)));
                            return RedirectToAction("OperationResultView", "Admin", new
                            {
                                area = "Admin",
                                succeeded = false,
                                message = "Erro inesperado. Não foi possível atualizar a senha. Por favor tente novamente"
                            });
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
