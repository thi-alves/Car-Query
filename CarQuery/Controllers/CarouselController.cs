using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class CarouselController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
