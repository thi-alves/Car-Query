using System.Data;
using System.Data.Common;
using System.Text.Json;
using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using CarQuery.ViewModels.CarouselViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace CarQuery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminCarouselController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICarouselRepository _carouselRepository;
        private readonly ICarRepository _carRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<AdminCarouselController> _logger;

        public AdminCarouselController(AppDbContext context, ICarouselRepository carouselRepository, ICarRepository carRepository, IImageRepository
            imageRepository, ILogger<AdminCarouselController> logger)
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
                if (pageIndex < 1)
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
                model.Action = "ListCarousels";

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                TempData["TotalCarousels"] = await _carouselRepository.CountCarousel();
                Carousel carousel = await _carouselRepository.GetCarouselById(id);
                CarouselViewModel carouselVm = new CarouselViewModel(carousel);

                if (carousel != null)
                {
                    return View(carouselVm);
                }
                Console.WriteLine("O carrossel é nulo!!");
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Não foi possível encontrar o carrossel selecionado. Ele pode ter sido deletado" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao abrir a página de edição" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarouselViewModel carouselVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == carouselVm.CarouselId)
                    {
                        List<CarouselSlide> carouselSlides = JsonSerializer.Deserialize<List<CarouselSlide>>(carouselVm.CarouselSlidesJson);

                        carouselVm.CarouselSlides.Clear();

                        Carousel carousel = await _carouselRepository.GetCarouselById(id);

                        if (carouselSlides != null)
                        {
                            foreach (var slide in carouselSlides)
                            {
                                slide.Car = await _carRepository.GetCarById(slide.CarId);
                                slide.Image = _imageRepository.GetImageById(slide.ImageId);
                                slide.CarouselId = carousel.CarouselId;
                                slide.Carousel = carousel;

                                carouselVm.CarouselSlides.Add(slide);
                            }

                            //Identificando quais slides devem ser excluidos
                            var slidesIdsToRemove = carousel.CarouselSlides
                                .Where(cs => !carouselVm.CarouselSlides
                                .Any(cv => cv.CarId == cs.CarId && cs.ImageId == cv.ImageId))
                                .ToList();

                            // Excluindo os slides
                            _context.CarouselSlide.RemoveRange(slidesIdsToRemove);

                            foreach (var slide in carouselVm.CarouselSlides)
                            {
                                var existingSlide = carousel.CarouselSlides
                                    .FirstOrDefault(cs => cs.CarId == slide.CarId && cs.ImageId == slide.ImageId);

                                //se true, significa que é um slide novo, logo cria-se um novo slide
                                if (existingSlide == null)
                                {
                                    CarouselSlide newSlide = new CarouselSlide();
                                    newSlide.CarId = slide.CarId;
                                    newSlide.Car = await _carRepository.GetCarById(slide.CarId);
                                    newSlide.Carousel = carousel;
                                    newSlide.ImageId = slide.ImageId;

                                    carousel.CarouselSlides.Add(newSlide);
                                }
                            }
                        }

                        int previousCarouselPosition = carousel.Position;
                        carousel.Title = carouselVm.Title;
                        carousel.Position = carouselVm.Position;
                        carousel.IsVisible = carouselVm.IsVisible;

                        bool operationSucceeded = await _carouselRepository.UpdateCarousel(carousel, previousCarouselPosition);

                        if (operationSucceeded) return RedirectToAction("OperationResultView", "Admin", new { succeeded = true, message = "O carrossel foi atualizado com sucesso" });
                    }
                }
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Não foi possível atualizar o carrossel. Erro inesperado" });
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Edit): {ExceptionType} erro ao atualizar carrossel no banco de dados", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao atualizar carrossel. Por favor tente novamente mais tarde." });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Edit): {ExceptionType} erro ao atualizar carrossel no banco de dados", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao atualizar carrossel. Por favor tente novamente mais tarde." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarouselController (Edit): {ExceptionType} erro inesperado ao atualizar carrossel", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao atualizar carrossel. Por favor tente novamente mais tarde." });
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
