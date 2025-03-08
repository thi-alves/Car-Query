using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface ICarRepository
    {
        Car GetCarById(int carId);

        Task UpdateCar(Car car);

        Task<bool> DeleteCar(int carId);

        Task<IEnumerable<Car>> SearchByModel(string model);
    }
}
