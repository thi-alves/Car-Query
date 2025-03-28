namespace CarQuery.ViewModels.CarouselSlidesViewModel
{
    public class CarouselSlideViewModel
    {
        public int CarId { get; set; }
        public string CarModel { get; set; }
        public string CarShortDescription { get; set; }
        public string ImgPath { get; set; }

        public CarouselSlideViewModel(int carId, string carModel, string carShortDescription, string imgPath)
        {
            CarId = carId;
            CarModel = carModel;
            CarShortDescription = carShortDescription;
            ImgPath = imgPath;
        }
    }
}
