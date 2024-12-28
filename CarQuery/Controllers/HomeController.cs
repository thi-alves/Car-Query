using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
