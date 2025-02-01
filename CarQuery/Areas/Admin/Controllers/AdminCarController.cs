using CarQuery.Data;
using CarQuery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CarQuery.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using ReflectionIT.Mvc.Paging;
using System.Numerics;
using System.IO;

namespace CarQuery.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AdminCarController : Controller
    {
        private string ServerPath { get; set; }
        private readonly AppDbContext _context;

        public AdminCarController(IWebHostEnvironment system, AppDbContext context)
        {
            ServerPath = system.WebRootPath;
            _context = context;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task <IActionResult> ListCars(string filter, int pageIndex = 1, string sort = "Model")
        {
            var result = _context.Car.Include(i => i.Images).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(m => m.Model.Contains(filter));
            }

            var model = await PagingList.CreateAsync(result, 10, pageIndex, sort, "Model");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Brand, Model, Year, Power, Drivetrain, Engine, " +
            "EnginePosition, TransmissionType, TopSpeed, Doors, Price, ShortDescription, FullDescription, " +
            "Images, VideoLink")] CarViewModel carViewModel)
        {
            if (ModelState.IsValid){

                Car car = new Car
                {
                    Brand = carViewModel.Brand,
                    Model = carViewModel.Model,
                    Year = carViewModel.Year,
                    Power = carViewModel.Power,
                    Drivetrain = carViewModel.Drivetrain,
                    Engine = carViewModel.Engine,
                    EnginePosition = carViewModel.EnginePosition,
                    TransmissionType = carViewModel.TransmissionType,
                    TopSpeed = carViewModel.TopSpeed,
                    Doors = carViewModel.Doors,
                    Price = carViewModel.Price,
                    ShortDescription = carViewModel.ShortDescription,
                    FullDescription = carViewModel.FullDescription,
                    VideoLink = carViewModel.VideoLink,

                    Images = new List<Image>()
                };

                Console.WriteLine("Marca do carro: " + car.Brand);

                await UploadFiles(carViewModel.Images, car);

                await _context.Car.AddAsync(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Success");
        }

        public async Task UploadFiles(List<IFormFile> files, Car car)
        {
            string folder = "\\ImgCar\\";
            string uploadImagePath = ServerPath + folder;
            
            if(files.Count > 0)
            {
                if (!Directory.Exists(uploadImagePath))
                {
                    Directory.CreateDirectory(uploadImagePath);
                }

                for(int i = 0; i < files.Count; i++)
                {
                    var newImageName = Guid.NewGuid().ToString() + files[i].FileName;

                    //O caminho "uploadImagePath" é necessário apenas para salvar a imagem no diretório coreto.
                    //Já para exibir a imagem, basta o nome da pasta dentro do wwwroot e o nome da imagem.
                    //Exemplo: ImgCar/4idd8bwjj
                    var imgPath = folder + newImageName;
                    imgPath = imgPath.Replace("\\", "/");

                    car.Images.Add(new Image { ImgPath = imgPath, Car = car});

                    using (var stream = System.IO.File.Create(uploadImagePath + newImageName))
                    {
                        files[i].CopyTo(stream);
                    }

                }
            }

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var car = _context.Car.Include(c => c.Images).FirstOrDefault(c => c.CarId == id);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarId, Brand, Model, Year, Power, Drivetrain, Engine, " +
            "EnginePosition, TransmissionType, TopSpeed, Doors, Price, ShortDescription, FullDescription, " +
            "Images, VideoLink")] Car car, string imgIndexDelete)
        {
            if(id != car.CarId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    int[] imgPosDelete = imgIndexDelete.Split(',')
                        .Select(int.Parse)
                        .ToArray();

                    //ordenando em ordem crescente
                    Array.Sort(imgPosDelete);

                    int total = car.Images.Count;

                    int adjust = 0;

                    for(int i = 0; i < imgPosDelete.Length; i++)
                    {

                        if (imgPosDelete[i] < total)
                        {
                            /*Subtrai adjust porque quando removemos um item da lista, o índice os outros items são alterados.
                             Como o array imgPosDelete foi ordenado de forma crescente, sempre que removermos um item, 
                            os próximos itens a serem removidos terão o índice decrementado*/
                            Console.WriteLine("Iteração: " + i);
                            Console.WriteLine("imgPostDelet value: " + imgPosDelete[i]);
                            Console.WriteLine("Valor do adjust: " + adjust);
                            Console.WriteLine("imgPath: " + car.Images[0].ImgPath);
                            string normalizedPath = Path.GetFullPath(car.Images[imgPosDelete[i]-adjust].ImgPath);
                            string imgPath = ServerPath + normalizedPath;

                            if (System.IO.File.Exists(imgPath))
                            {
                                Console.WriteLine("Caminho da imagem deletada: " + imgPath);
                                System.IO.File.Delete(imgPath);

                                car.Images.RemoveAt(imgPosDelete[i]-adjust);
                            }
                            
                            adjust++;
                        }
                        else
                        {
                            Console.WriteLine("É maior que o total");
                        }
                    
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return RedirectToAction("Success");
        }

        
        public bool CarExists(int id)
        {
            return _context.Car.Any(c => c.CarId == id);

        }
    }
}
