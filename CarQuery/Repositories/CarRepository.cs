using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;

namespace CarQuery.Repositories
{
    public class CarRepository : ICarRepository
    {

        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Car> Car { get; set; }


        public Car GetCarById(int carId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Car> GetCars()
        {
            throw new NotImplementedException();

        }

        public void Edit()
        {
            
        }

        public void Delete(int CarId)
        {

        }
    }
}
