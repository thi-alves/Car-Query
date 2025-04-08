using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReflectionIT.Mvc.Paging;

namespace CarQuery.Controllers
{
    public class CarController : Controller
    {
        public readonly ICarRepository _carRepository;
        public readonly AppDbContext _context;

        public CarController(ICarRepository carRepository, AppDbContext context)
        {
            _carRepository = carRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> ListCars(string filter, int minPower = 0, int maxPower = 0, int minYear = 0, int maxYear = 0, double minPrice = 0, double maxPrice = 0, int pageIndex = 1, string sort = "Model", List<Car> filteredCars = null)
        {
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }

                var result = _context.Car.Include(i => i.Images).AsQueryable();

                result = ApplyFilters(result, filter, minPower, maxPower, minYear, maxYear, minPrice, maxPrice);

                var model = await PagingList.CreateAsync(result, 10, pageIndex, sort, "Model");

                model.RouteValue = new RouteValueDictionary 
                {
                    { "filter", filter },
                    { "minPower", minPower },
                    { "maxPower", maxPower },
                    { "minYear", minYear },
                    { "maxYear", maxYear },
                    { "minPrice", minPrice },
                    { "maxPrice", maxPrice }
                };
                model.Action = "ListCars";

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                TempData["ErrorMessage"] = "Erro inesperado, por favor tente novamente";
                return RedirectToAction("FilterSearch");
            }
        }

        [HttpGet]
        public IActionResult FilterSearch()
        {
            return View();
        }

        [HttpPost]
        public IQueryable<Car> ApplyFilters(IQueryable<Car> query, string filter, int minPower, int maxPower, int minYear, int maxYear, double minPrice, double maxPrice)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(c => (c.Brand + " " + c.Model).Contains(filter));
            }
            if (minPower > 0)
            {
                query = query.Where(c => c.Power >= minPower);
            }
            if (maxPower != 0)
            {
                query = query.Where(c => c.Power <= maxPower);
            }
            if (minYear != 0)
            {
                query = query.Where(c => c.ModelYear >= minYear);
            }
            if (maxYear != 0)
            {
                query = query.Where(c => c.ModelYear <= maxYear);
            }
            if (minPrice != 0)
            {
                query = query.Where(c => c.Price >= minPrice);
            }
            if (maxPrice != 0)
            {
                query = query.Where(c => c.Price <= maxPrice);
            }
            
            return query;
        }

        public async Task<IActionResult> CarDetailsPage(int id)
        {
            Console.WriteLine("Imprimindo o ID: " + id);
            Car car = await _carRepository.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }
    }
}
