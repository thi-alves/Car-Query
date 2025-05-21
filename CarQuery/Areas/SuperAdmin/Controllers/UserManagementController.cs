using System.Data.Common;
using CarQuery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UserManagementController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext context, ILogger<UserManagementController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
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
    }

}
