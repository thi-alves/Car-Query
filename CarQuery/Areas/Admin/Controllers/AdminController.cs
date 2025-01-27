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
    }
}
