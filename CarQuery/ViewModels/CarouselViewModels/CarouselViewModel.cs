using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CarQuery.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace CarQuery.ViewModels.CarouselViewModels
{
    public class CarouselViewModel
    {
        [Key]
        public int CarouselId { get; set; }

        [Required(ErrorMessage = "Este campo deve ser preenchido")]
        [MaxLength(100, ErrorMessage = "Este campo deve ter no máximo 100 caracteres")]
        public string Title { get; set; }
        public List<CarouselSlide> CarouselSlides { get; set; } = new List<CarouselSlide>();
        public short Position { get; set; }
        public bool IsVisible { get; set; }
        public string CarouselSlidesJson { get; set; }

        public CarouselViewModel()
        {

        }
        public CarouselViewModel(string title, List<CarouselSlide> carouselSlides, short position, bool isVisible)
        {
            Title = title;
            CarouselSlides = carouselSlides;
            Position = position;
            IsVisible = isVisible;
        }
        public CarouselViewModel(Carousel carousel)
        {
            CarouselId = carousel.CarouselId;
            Title = carousel.Title;
            CarouselSlides = carousel.CarouselSlides;
            Position = carousel.Position;
            IsVisible = carousel.IsVisible;
            CarouselSlidesJson = JsonSerializer.Serialize(carousel.CarouselSlides);
        }

    }
}
