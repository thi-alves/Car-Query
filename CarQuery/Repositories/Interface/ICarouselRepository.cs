using CarQuery.Models;

namespace CarQuery.Repositories.Interface
{
    public interface ICarouselRepository
    {

        Task<int> CountCarousel();

        Task<bool> UpdateCarouselPosition(int position);

        Task<bool> CreateCarousel(Carousel carousel);
        Task<bool> DeleteCarousel(int carouselId);
    }
}
