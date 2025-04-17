using CarQuery.Data;
using CarQuery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using ReflectionIT.Mvc.Paging;
using System.Numerics;
using System.IO;
using CarQuery.Repositories;
using CarQuery.Repositories.Interface;
using System.Data.Common;
using System.Data;
using CarQuery.ViewModels.CarViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CarQuery.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminCarController : Controller
    {
        private string ServerPath { get; set; }
        private readonly AppDbContext _context;
        private readonly ICarRepository _carRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger <AdminCarController> _logger;

        public AdminCarController(IWebHostEnvironment system, AppDbContext context, ICarRepository carRepository, IImageRepository imageRepository, ILogger <AdminCarController> logger)
        {
            ServerPath = system.WebRootPath;
            _context = context;
            _carRepository = carRepository;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListCars(string filter, int pageIndex = 1, string sort = "Model")
        {
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }

                var result = _context.Car.Include(i => i.Images).AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    result = result.Where(m => m.Model.Contains(filter));
                }

                var model = await PagingList.CreateAsync(result, 10, pageIndex, sort, "Model");
                model.RouteValue = new RouteValueDictionary { { "filter", filter } };
                model.Action = "ListCars";

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao listar carros. Por favor tente novamente mais tarde." });
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Brand, Model, BodyStyle, ManufacturingYear, ModelYear, Power, Drivetrain, Displacement, " +
            "Valves, FuelType, Aspiration, Cylinders, CylinderConfiguration, EnginePosition, TransmissionType, TopSpeed, Doors, Price, ShortDescription, FullDescription, " +
            "Images, VideoLink")] CarViewModel carViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Car car = new Car(carViewModel);

                    Console.WriteLine("Marca do carro: " + car.Brand);

                    await UploadFiles(carViewModel.Images, car);

                    bool result = await _carRepository.AddCar(car);

                    return RedirectToAction("OperationResultView", "Admin", new
                    {
                        succeeded = result,
                        message = result ? "Carro adicionado com sucesso!" : "Erro ao adicionar carro"
                    });
                }
                return View(carViewModel);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "AdminCarController (Add): {ExceptionType} erro ao salvar o carro no banco de dados", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao salvar o carro no banco de dados. Por favor tente novamente mais tarde." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarController (Add): {ExceptionType} erro inesperado ao adicionar carro", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao adicionar carro. Por favor tente novamente mais tarde." });
            }
        }

        public async Task UploadFiles(List<IFormFile> files, Car car)
        {
            string folder = "\\ImgCar\\";
            string uploadImagePath = ServerPath + folder;

            if (files.Count > 0)
            {
                if (!Directory.Exists(uploadImagePath))
                {
                    Directory.CreateDirectory(uploadImagePath);
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var newImageName = Guid.NewGuid().ToString() + files[i].FileName;

                    //O caminho "uploadImagePath" é necessário apenas para salvar a imagem no diretório coreto.
                    //Já para exibir a imagem, basta o nome da pasta dentro do wwwroot e o nome da imagem.
                    //Exemplo: ImgCar/4idd8bwjj
                    var imgPath = folder + newImageName;
                    imgPath = imgPath.Replace("\\", "/");

                    car.Images.Add(new Image { ImgPath = imgPath, Car = car });

                    using (var stream = System.IO.File.Create(uploadImagePath + newImageName))
                    {
                        files[i].CopyTo(stream);
                    }
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var car = await _context.Car.Include(c => c.Images).FirstOrDefaultAsync(c => c.CarId == id);

                if (car != null) return View(car);

                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Não foi possível encontrar o carro selecionado. Ele pode ter sido deletado" });

            }
            catch (Exception)
            {
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao abrir a página de edição" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarId, Brand, Model, BodyStyle, ManufacturingYear, ModelYear, Power, Drivetrain, Displacement, " +
            "Valves, FuelType, Aspiration, Cylinders, CylinderConfiguration, EnginePosition, TransmissionType, TopSpeed, Doors, Price, ShortDescription, FullDescription, " +
            "Images, VideoLink")] Car editedCar, List<IFormFile> newImages, string imgIdsDelete = "0")
        {
            if (id != editedCar.CarId)
            {
                Console.WriteLine("O Id é diferente");
                Console.WriteLine("id: " + id + "-- CarId: " + editedCar.CarId);
                return NotFound();
            }

            //como não fizemos Bind no Images, a model editedCar vem sem elas, então é necessário reatribui-las para o caso termos que retornar para o edit e imprimir as imagens (em caso de erro)
            var images = await _imageRepository.GetImagesByCarId(editedCar.CarId);

            editedCar.Images = images;

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state é válida");
                try
                {
                    int[] imgIdDelete = imgIdsDelete.Split(',')
                        .Select(int.Parse)
                        .ToArray();

                    //if true existe imagens a serem deletadas
                    if (imgIdDelete[0] != 0)
                    {
                        int totalCarImg = editedCar.Images.Count;
                        int totalImgDelete = imgIdDelete.Count();

                        //para garantir que reste pelo menos uma imagem associada ao carro
                        if (totalImgDelete < totalCarImg || newImages.Count >= 1)
                        {
                            //ordenando em ordem crescente
                            Array.Sort(imgIdDelete);

                            for (int i = 0; i < imgIdDelete.Length; i++)
                            {
                                Console.WriteLine("Iteração: " + i);

                                Image img = images.FirstOrDefault(c => c.ImageId == imgIdDelete[i]);

                                if (img != null)
                                {
                                    Console.WriteLine("imgPath: " + img.ImgPath);
                                    string normalizedPath = (img.ImgPath).Replace("/", "\\");
                                    string imgPath = ServerPath + normalizedPath;

                                    Console.WriteLine("Complete Image Path: " + imgPath);

                                    if (System.IO.File.Exists(imgPath))
                                    {
                                        Console.WriteLine("File Exists");
                                        Console.WriteLine("Caminho da imagem deletada: " + imgPath);

                                        //deletando a imagem do servidor
                                        System.IO.File.Delete(imgPath);

                                        //deletando no banco de dados
                                        await _imageRepository.DeleteImageById(imgIdDelete[i]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("File doesn't exists!!");

                                    }
                                }
                            }
                        }
                        else
                        {
                            return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Não é possível deletar todas as imagens. O carro deve possuir pelo menos uma imagem" });
                        }

                        _context.Entry(editedCar).Reload();
                    }

                    await UploadFiles(newImages, editedCar);

                    bool result = await _carRepository.UpdateCar(editedCar);

                    return RedirectToAction("OperationResultView", "Admin", new
                    {
                        succeeded = result,
                        message = result ? "O carro foi atualizado com sucesso" : "Não foi possível atualizar as informações do carro. Por favor tente novamente"
                    });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CarExists(id))
                    {
                        _logger.LogError(ex, "AdminCarController (Edit): {ExceptionType} Não existe um carro com o id especificado", ex.GetType().Name);
                        return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Não existe um carro com o id especificado" });
                    }
                    else
                    {
                        _logger.LogError(ex, "AdminCarController (Edit): {ExceptionType} falha ao atualizar as informações do carro", ex.GetType().Name);
                        return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar atualizar as informações do carro" });
                    }
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex, "AdminCarController (Edit): {ExceptionType} falha ao atualizar as informações do carro", ex.GetType().Name);
                    return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar atualizar as informações do carro" });
                }
                catch (FormatException)
                {
                    return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro. Não é permitido caracteres e/ou espaços na entrada para deletar imagens" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AdminCarController (Edit): {ExceptionType} falha ao atualizar as informações do carro", ex.GetType().Name);
                    return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar atualizar as informações do carro" });
                }
            }
            Console.WriteLine("Model state is invalid!!");

            return View(editedCar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("ID RECEBIDO: " + id);
                bool result = await _carRepository.DeleteCar(id);

                return RedirectToAction("OperationResultView", "Admin", new
                {
                    succeeded = result,
                    message = result ? "O carro foi removido com sucesso" : "Não foi possível remover o carro. Por favor tente novamente."
                });
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, "AdminCarController (Delete): {ExceptionType} erro ao remover o carro", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar remover o carro. Por favor tente novamente mais tarde." });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "AdminCarController (Delete): {ExceptionType} erro ao remover o carro", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar remover o carro. Por favor tente novamente mais tarde." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarController (Delete): {ExceptionType} erro inesperado ao remover o carro", ex.GetType().Name);
                return RedirectToAction("OperationResultView", "Admin", new { succeeded = false, message = "Erro ao tentar remover o carro. Por favor tente novamente mais tarde." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchByModel([FromQuery] string model)
        {
            try
            {
                Console.WriteLine("Model recebida: " + model);
                if (!string.IsNullOrEmpty(model))
                {
                    IEnumerable<Car> cars = await _carRepository.SearchByModel(model);

                    return Ok(cars);
                }
                Console.WriteLine("Lista vazia");
                IEnumerable<Car> emptyList = Enumerable.Empty<Car>();
                return Ok(emptyList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminCarController (SearchByModel): {ExceptionType} erro inesperado ao buscar por carros", ex.GetType().Name);
                IEnumerable<Car> emptyList = Enumerable.Empty<Car>();
                return Ok(emptyList);
            }
        }

        public bool CarExists(int id)
        {
            return _context.Car.Any(c => c.CarId == id);
        }
    }
}
