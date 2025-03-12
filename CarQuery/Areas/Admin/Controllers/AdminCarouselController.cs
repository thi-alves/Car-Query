using System.Data.Common;
using System.Text.Json;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CarQuery.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCarouselController : Controller
    {
        private readonly ICarouselRepository _carouselRepository;
        private readonly ICarRepository _carRepository;
        private readonly IImageRepository _imageRepository;

        public AdminCarouselController(ICarouselRepository carouselRepository, ICarRepository carRepository, IImageRepository
            imageRepository)
        {
            _carouselRepository = carouselRepository;
            _carRepository = carRepository;
            _imageRepository = imageRepository;
        }

        public async Task<IActionResult> Create()
        {
            TempData["TotalCarousels"] = await _carouselRepository.CountCarousel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Carousel carousel, string jsonSlides)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Console.WriteLine("Model state está válida");
                    var slides = JsonSerializer.Deserialize<List<CarouselSlide>>(jsonSlides);

                    if (slides != null)
                    {
                        foreach (var slide in slides)
                        {
                            slide.Car = await _carRepository.GetCarById(slide.CarId);
                            slide.Image = _imageRepository.GetImageById(slide.ImageId);
                            slide.Carousel = carousel;

                            carousel.CarouselSlides.Add(slide);
                        }

                        bool operationSucceeded = await _carouselRepository.CreateCarousel(carousel);

                        if (operationSucceeded) return RedirectToAction("Index", "Admin");
                    }
                }
                Console.WriteLine("Model state is invalid");

                carousel.CarouselSlides.Clear();
                TempData["TotalCarousels"] = await _carouselRepository.CountCarousel();
                return View(carousel);
            }
            catch (DbException)
            {
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar criar Carousel. Por favor tente novamente mais tarde." });

            }
            catch (Exception)
            {
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar criar Carousel. Por favor tente novamente mais tarde." });
            }
        }
    }
}
