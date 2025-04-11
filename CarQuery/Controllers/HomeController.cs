using System.Net.Mail;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarouselRepository _carouselRepository;

        public HomeController(ICarouselRepository carouselRepository)
        {
            _carouselRepository = carouselRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carousels = await _carouselRepository.GetAllVisibleCarouselsToDisplay();
            return View(carousels);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Feedback()
        {
            return View();
        }
    }
}
