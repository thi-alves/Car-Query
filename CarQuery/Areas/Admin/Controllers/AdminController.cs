using CarQuery.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OperationResultView(OperationResult operationResult)
        {
            return View(operationResult);
        }
    }
}
