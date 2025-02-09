using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface ICarRepository
    {
        Car GetCarById(int carId);

        void UpdateCar(Car car);

        void DeleteCar(int carId);
    }
}
