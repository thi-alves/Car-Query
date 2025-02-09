using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface IImageRepository
    {
        Task<List<Image>> GetImagesByCarId(int carId);
        Image GetImageById(int imgId);
        Task DeleteImageById(int imageId);
    }
}
