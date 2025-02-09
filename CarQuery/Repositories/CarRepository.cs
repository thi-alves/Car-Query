using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
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

        public async void UpdateCar(Car car)
        {
            _context.Update(car);
            await _context.SaveChangesAsync();
        }

        public async void DeleteCar(int carId)
        {
            Car car = _context.Car
                .Include(c => c.Images)
                .FirstOrDefault(c => c.CarId == carId);

            if (car != null)
            {
                _context.Car.Remove(car);

                await _context.SaveChangesAsync();
            }
        }
    }
}
