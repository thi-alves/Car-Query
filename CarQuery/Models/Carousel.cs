using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using CarQuery.ViewModels;

namespace CarQuery.Models
{
    public class Carousel
    {
        [Key]
        public int CarouselId { get; set; }

        [Required(ErrorMessage = "Este campo deve ser preenchido")]
        [MaxLength(100, ErrorMessage = "Este campo deve ter no máximo 100 caracteres")]
        public string Title { get; set; }
        public List<CarouselSlide> CarouselSlides { get; set; } = new List<CarouselSlide>();
        public short Position { get; set; }
        public bool IsVisible { get; set; }
   
        public Carousel()
        {

        }
        public Carousel(string title, List<CarouselSlide> carouselSlides, short position, bool isVisible)
        {
            Title = title;
            CarouselSlides = carouselSlides;
            Position = position;
            IsVisible = isVisible;
        }
    }
}
