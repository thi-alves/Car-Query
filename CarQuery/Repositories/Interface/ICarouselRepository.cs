using CarQuery.Models;
using CarQuery.ViewModels.CarouselViewModels;

namespace CarQuery.Repositories.Interface
{
    public interface ICarouselRepository
    {

        Task<int> CountCarousel();
        Task<bool> UpdateCarouselPosition(int newPosition, int previousPosition);
        Task<bool> CreateCarousel(Carousel carousel);
        Task<Carousel> GetCarouselById(int id);
        Task<bool> UpdateCarousel(Carousel carousel, int previousPosition);
        Task<bool> DeleteCarousel(int carouselId);
        Task<bool> DeleteAllCarouselsSlidesByImage(Image img);
        Task<List<CarouselDisplayViewModel>> GetAllVisibleCarouselsToDisplay();
    }
}
