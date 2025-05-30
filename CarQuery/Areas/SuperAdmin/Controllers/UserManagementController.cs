using System.Data.Common;
using CarQuery.Data;
using CarQuery.Services;
using CarQuery.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Polly;
using ReflectionIT.Mvc.Paging;

namespace CarQuery.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementController> _logger;
        private readonly IPasswordUpdateService _passwordUpdateService;

        public UserManagementController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext context, ILogger<UserManagementController> logger, IPasswordUpdateService passwordUpdateService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _passwordUpdateService = passwordUpdateService;
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers(string filter, int pageIndex = 1, string sort = "UserName")
        {
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }

                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

                if (adminRole == null)
                {
                    //retorna lista paginada vazia
                    var emptyQuery = Enumerable.Empty<IdentityUser>().AsQueryable();

                    var emptyModel = await PagingList.CreateAsync(emptyQuery, 10, pageIndex, sort, "UserName");
                    emptyModel.RouteValue = new RouteValueDictionary { { "filter", filter } };
                    emptyModel.Action = "ListUsers";

                    return View(emptyModel);
                }

                var result = from user in _context.Users
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             where userRole.RoleId == adminRole.Id
                             select user;

                if (!string.IsNullOrEmpty(filter))
                {
                    result = result.Where(u => u.UserName.Contains(filter) || u.Email.Contains(filter));
                }

                var model = await PagingList.CreateAsync(result, 10, pageIndex, sort, "UserName");
                model.RouteValue = new RouteValueDictionary { { "filter", filter } };
                model.Action = "ListUsers";

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserManagementController (ListUsers): {ExceptionType} Erro ao listar usuários", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { area = "Admin", succeeded = false, message = "Erro ao listar usuários. Por favor tente novamente mais tarde." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("OperationResultView", "Admin", new
                        {
                            area = "Admin",
                            succeeded = true,
                            message = "O usuário foi deletado com sucesso"
                        });
                    }
                }
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Não foi possível deletar o usuário. Por favor tente novamente."
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "UserManagementController (DeleteUser): {ExceptionType} Erro ao deletar usuário", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível deletar o usuário. Por favor tente novamente."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    EditUserViewModel userVm = new EditUserViewModel(user);
                    return View(userVm);
                }
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível encontrar o usuário. Por favor tente novamente"
                });
            }
            catch (Exception)
            {
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível encontrar o usuário. Por favor tente novamente"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel userVm)
        {
            try
            {
                //serve para remover os erros de validação desses campos, que ocorreriam ao testar ModelState.IsValid no caso de o SuperAdmin não alterar a senha, fazendo com que os atributos abaixo fossem nulos ou vazios
                if (string.IsNullOrEmpty(userVm.NewPassword))
                {
                    ModelState.Remove(nameof(userVm.NewPassword));
                    ModelState.Remove(nameof(userVm.ConfirmNewPassword));
                }

                var user = await _userManager.FindByIdAsync(userVm.UserId);

                if (user != null)
                {
                    //Criando usuário temporário para verificar se userName email atendem os requisitos configurados
                    var tempUser = new IdentityUser { UserName = userVm.UserName, Email = userVm.Email };
                    var userValidation = await _userManager.UserValidators[0].ValidateAsync(_userManager, tempUser);

                    if (!userValidation.Succeeded)
                    {
                        /* Caso o email ou nome do usuário não mude, o UserValidator entende que eles estão duplicados pois compara o tempUser 
                         * com o usuário (que queremos atualizar) registrado no banco de dados. Esse código serve para lidar com essa possibilidade*/
                        bool emailDuplicated = userValidation.Errors.Any(e => e.Code == "DuplicateEmail");
                        bool userNameDuplicated = userValidation.Errors.Any(e => e.Code == "DuplicateUserName");

                        List<string> ignoreErrors = new List<string>();
                        //caso verdadeiro, significa que o único email duplicado é o do próprio usuário que queremos atualizar
                        if (emailDuplicated && userVm.Email.Equals(user.Email))
                        {
                            ignoreErrors.Add("DuplicateEmail");
                        }

                        if (userNameDuplicated && userVm.UserName.Equals(user.UserName))
                        {
                            ignoreErrors.Add("DuplicateUserName");
                        }

                        foreach (var error in userValidation.Errors)
                        {
                            if (!ignoreErrors.Contains(error.Code))
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }

                        if (!ModelState.IsValid)
                        {
                            return View(userVm);
                        }
                    }

                    user.UserName = userVm.UserName;
                    user.Email = userVm.Email;

                    var updateUserResult = await _userManager.UpdateAsync(user);

                    if (!updateUserResult.Succeeded)
                    {
                        return RedirectToAction("OperationResultView", "Admin", new
                        {
                            area = "Admin",
                            succeeded = false,
                            message = "Erro inesperado. Não foi possível atualizar os dados do usuário. Por favor tente novamente"
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(userVm.NewPassword))
                    {
                        var (result, errors) = await _passwordUpdateService.PasswordUpdateAsync(user, userVm.NewPassword);

                        if (!result)
                        {
                            if (errors.Count == 1 && errors[0].Equals("Internal error"))
                            {
                                return RedirectToAction("OperationResultView", "Admin", new
                                {
                                    area = "Admin",
                                    succeeded = false,
                                    message = "Erro inesperado. Não foi possível atualizar a senha do usuário. Por favor tente novamente"
                                });
                            }
                            else
                            {
                                foreach (var error in errors)
                                {
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                return View(userVm);
                            }
                        }
                    }
                    return RedirectToAction("OperationResultView", "Admin", new
                    {
                        area = "Admin",
                        succeeded = true,
                        message = "Os dados do usuário foram atualizados com sucesso!"
                    });
                }
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível atualizar a senha do usuário. Por favor tente novamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserManagement (EditUser): {ExceptionType} Erro inesperado ao atualizar dados do usuário {UserId}", ex.GetType().Name, userVm.UserId);
                return RedirectToAction("OperationResultView", "Admin", new
                {
                    area = "Admin",
                    succeeded = false,
                    message = "Erro inesperado. Não foi possível atualizar os dados do usuário. Por favor tente novamente"
                });
            }
        }
    }

}
