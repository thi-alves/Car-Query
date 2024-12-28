namespace CarQuery.Models
{
    public class Carousel
    {
        public int CarouselId { get; set; }
        public string Title { get; set; }
        public List<Car> Cars { get; set; }
        public List<Image> Images { get; set; }

        public Carousel(string title, List<Car> cars, List<Image> images)
        {
            Title = title;
            Cars = cars;
            Images = images;

        }
    }
}
