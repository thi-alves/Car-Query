using System.Linq.Expressions;
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

        public async Task<bool> AddCar(Car car)
        {
            await _context.Car.AddAsync(car);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
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
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCar(int carId)
        {
            Car car = await _context.Car
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CarId == carId);

            if (car != null)
            {
                _context.Car.Remove(car);

                int rowsAffected = await _context.SaveChangesAsync();
                return rowsAffected > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Car>> SearchByModel(string model)
        {

            if (!string.IsNullOrEmpty(model))
            {
                var car = await _context.Car.Include(c => c.Images)
                .Where(c => c.Model.StartsWith(model))
                .ToListAsync();

                if (car != null) return car;
            }
            return Enumerable.Empty<Car>();
        }
    }
}
