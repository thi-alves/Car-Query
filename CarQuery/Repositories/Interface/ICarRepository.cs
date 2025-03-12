using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface ICarRepository
    {
        Task<bool> AddCar(Car car);

        Task<Car> GetCarById(int carId);

        Task<bool> UpdateCar(Car car);

        Task<bool> DeleteCar(int carId);

        Task<IEnumerable<Car>> SearchByModel(string model);
    }
}
