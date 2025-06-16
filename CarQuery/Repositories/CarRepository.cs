using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CarQuery.Repositories
{
    public class CarRepository : ICarRepository
    {
        private string ServerPath { get; set; }
        private readonly AppDbContext _context;

        public CarRepository(IWebHostEnvironment system, AppDbContext context)
        {
            ServerPath = system.WebRootPath;
            _context = context;
        }

        public async Task<bool> AddCar(Car car)
        {
            await _context.Car.AddAsync(car);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Car> GetCarById(int carId)
        {
            Car car = await _context.Car
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CarId == carId);

            return car;
        }

        public async Task<bool> UpdateCar(Car car)
        {
            if (car == null)
            {
                return false;
            }
            _context.Update(car);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCar(int carId)
        {
            Car car = await _context.Car
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CarId == carId);

            if (car != null)
            {
                foreach (var img in car.Images)
                {
                    string normalizedPath = (img.ImgPath).Replace("/", "\\");
                    string imgPath = ServerPath + normalizedPath;

                    if (System.IO.File.Exists(imgPath))
                    {
                        //deletando a imagem do servidor
                        System.IO.File.Delete(imgPath);
                    }
                }
                _context.Car.Remove(car);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Car>> SearchByModel(string model)
        {
            if (string.IsNullOrEmpty(model))
            {
                return Enumerable.Empty<Car>();
            }

            var car = await _context.Car.Include(c => c.Images)
            .Where(c => c.Model.StartsWith(model))
            .ToListAsync();

            return car;
        }

    }
}

