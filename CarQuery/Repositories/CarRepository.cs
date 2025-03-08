using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CarQuery.Repositories
{
    public class CarRepository : ICarRepository
    {

        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public Car GetCarById(int carId)
        {
            return _context.Car
                .Include(c => c.Images)
                .FirstOrDefault(c => c.CarId == carId);
        }

        public async Task UpdateCar(Car car)
        {
            _context.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCar(int carId)
        {
            Car car = _context.Car
                .Include(c => c.Images)
                .FirstOrDefault(c => c.CarId == carId);

            if (car != null)
            {
                _context.Car.Remove(car);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Car>> SearchByModel(string model)
        {
            try
            {
                if (string.IsNullOrEmpty(model))
                {
                    return Enumerable.Empty<Car>();
                }

                var car = await _context.Car.Include(c => c.Images)
                    .Where(c => c.Model.StartsWith(model))
                    .ToListAsync();

                if (car != null) return car;

                throw new Exception("O carro não foi encontrado");

            }
            catch (Exception)
            {
                Console.WriteLine("Erro");
                return Enumerable.Empty<Car>();
            }
        }
    }
}
