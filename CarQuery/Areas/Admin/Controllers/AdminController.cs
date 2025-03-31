using CarQuery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OperationResultView(bool succeeded, string message)
        {
            OperationResult operationResult = new OperationResult
            {
                Succeeded = succeeded,
                Message = message
            };

            return View(operationResult);
        }
    }
}
