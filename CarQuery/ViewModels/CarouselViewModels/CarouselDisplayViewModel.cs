using CarQuery.ViewModels.CarouselSlidesViewModel;

namespace CarQuery.ViewModels.CarouselViewModels
{
    public class CarouselDisplayViewModel
    {
        public string Title { get; set; }
        public List<CarouselSlideViewModel>  CarouselSlides{ get; set; }

        public CarouselDisplayViewModel(string title)
        {
            Title = title;
            CarouselSlides = new List<CarouselSlideViewModel>();
        }

    }
}
