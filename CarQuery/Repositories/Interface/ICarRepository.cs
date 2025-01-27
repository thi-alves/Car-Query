using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface ICarRepository
    {
        public IEnumerable<Car> Car { get; set; }

        Car GetCarById(int carId);

        public IEnumerable<Car> GetCars();
    }
}
