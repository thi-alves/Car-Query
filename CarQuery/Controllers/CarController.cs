using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Controllers
{
    public class CarController : Controller
    {
        public readonly ICarRepository _carRepository;

        public CarController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult CarDetailsPage(int id)
        {
            Console.WriteLine("Imprimindo o ID: " + id);
            Car car = _carRepository.GetCarById(id);
            if(car == null)
            {
                return NotFound();
            }
            return View(car);
        }
    }
}
