using System.Data;
using System.Data.Common;
using System.Text.Json;
using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace CarQuery.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCarouselController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICarouselRepository _carouselRepository;
        private readonly ICarRepository _carRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger <AdminCarouselController> _logger;

        public AdminCarouselController(AppDbContext context, ICarouselRepository carouselRepository, ICarRepository carRepository, IImageRepository
            imageRepository, ILogger <AdminCarouselController> logger)
        {
            _context = context;
            _carouselRepository = carouselRepository;
            _carRepository = carRepository;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        public async Task<IActionResult> ListCarousels(string filter, int pageIndex = 1, string sort = "Position")
        {
            try
            {
                if(pageIndex < 1)
                {
                    pageIndex = 1;
                }

                var result = _context.Carousel.Include(c => c.CarouselSlides).ThenInclude(cs => cs.Image).AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    result = result.Where(m => m.Title.Contains(filter));
                }

                var model = await PagingList.CreateAsync(result, 10, pageIndex, sort, "Title");
                model.RouteValue = new RouteValueDictionary { { "filter", filter } };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao listar carousels. Por favor tente novamente mais tarde." });
            }
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

                        if (operationSucceeded) return RedirectToAction("OperationResultView", "Admin", new { succeeded = true, message = "O carrossel foi criado com sucesso" });
                    }
                }
                Console.WriteLine("Model state is invalid");

                carousel.CarouselSlides.Clear();
                TempData["TotalCarousels"] = await _carouselRepository.CountCarousel();
                return View(carousel);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Create): {ExceptionType} erro ao adicionar carrossel no banco de dados", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao criar carrossel. Por favor tente novamente mais tarde." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Create): {ExceptionType} erro inesperado ao criar o carrossel", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao criar carrossel. Por favor tente novamente mais tarde." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _carouselRepository.DeleteCarousel(id);

                return RedirectToAction("OperationResultView", "Admin", new
                {
                    succeeded = result,
                    message = result ? "O carrossel foi deletado com sucesso" : "Não foi possível deletar o carrossel. Por favor tente novamente."
                });
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Delete): {ExceptionType} erro ao deletar carrossel", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao deletar carrossel. Por favor tente novamente mais tarde." });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Delete): {ExceptionType} erro ao deletar carrossel", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao deletar carrossel. Por favor tente novamente mais tarde." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Delete): {ExceptionType} erro inesperado ao deletar carrossel", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao deletar carrosel. Por favor tente novamente mais tarde." });
            }
        }
    }
}
