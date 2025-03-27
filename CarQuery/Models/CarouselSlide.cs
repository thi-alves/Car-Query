using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarQuery.Models
{
    public class CarouselSlide
    {
        [Key]
        public int CarouselSlideId { get; set; }
        public int CarouselId { get; set; }
        [JsonIgnore]
        public Carousel Carousel {get; set;}
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }

        public CarouselSlide()
        {

        }

        public CarouselSlide(Car car, Image image)
        {
            CarId = car.CarId;
            Car = car;
            Image = image;
        }
    }
}
