using CarQuery.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class CarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult CarDetailsPage(Car car)
        {
            return View(car);
        }
    }
}
