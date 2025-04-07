using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarQuery.ViewModels.CarViewModels;

namespace CarQuery.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Informe a marca do veículo")]
        [MinLength(1, ErrorMessage = "A marca do carro deve ter no mínimo {1} caracteres")]
        [MaxLength(30, ErrorMessage = "A marca do carro deve deve ter no máximo {1} caracteres")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "informe o modelo do veículo")]
        [MinLength(1, ErrorMessage = "O nome do modelo deve ter no mínimo {1} caracteres")]
        [MaxLength(100, ErrorMessage = "O nome do modelo deve ter no máximo {1} caracteres")]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Informe o ano de fabricação do veículo")]
        [Display(Name = "Manufacturing Year")]
        public int ManufacturingYear { get; set; }

        [Required(ErrorMessage = "Informe o ano do modelo do veículo")]
        [Display(Name = "Model Year")]
        public int ModelYear { get; set; }

        [Required(ErrorMessage = "Informe a potência do veículo")]
        [Display(Name = "Power")]
        public short Power { get; set; }

        [Required(ErrorMessage = "Informe a tração do carro")]
        [MaxLength(10, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Drivetrain")]
        public string Drivetrain { get; set; }

        [Required(ErrorMessage = "Especifique o motor do carro")]
        [MaxLength(20, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Engine")]
        public string Engine { get; set; }

        [Required(ErrorMessage = "Informme a posição do motor")]
        [MaxLength(8, ErrorMessage = "A posição do motor deve ter no máximo {1} caracteres")]
        [Display(Name = "Engine position")]
        public string EnginePosition { get; set; }

        [Required(ErrorMessage = "Informe o tipo de transmissão")]
        [MaxLength(30, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Transmission type")]
        public string TransmissionType { get; set; }

        [Required(ErrorMessage = "Informe a velocidade máxima do veículo")]
        [Display(Name = "Top speed")]
        public short TopSpeed { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de portas")]
        [Display(Name = "Doors")]
        public short Doors { get; set; }

        [Required(ErrorMessage = "Informe o preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição curta")]
        [MinLength(20, ErrorMessage = "A descrição curta precisa ter no mínimo {1} caracteres")]
        [MaxLength(300, ErrorMessage = "A descrição curta deve ter no máximo {1} caracteres")]
        [Display(Name = "Short description")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição completa")]
        [MinLength(20, ErrorMessage = "A descrição precisa ter no mínimo {1} caracteres")]
        [MaxLength(2100, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Full description")]
        public string FullDescription { get; set; }

        public List<Image> Images { get; set; }
        public string VideoLink { get; set; }


        public Car()
        {

        }
        public Car(string brand, string model, int manufacturingYear, int modelYear, short power, string drivetrain, string engine,
            string enginePosition, string transmissionType, short topspeed, short doors, double price,
            string shortDescription, string fullDescription, List<Image> images, string videoLink
            )
        {
            Brand = brand;
            Model = model;
            ManufacturingYear = manufacturingYear;
            ModelYear = modelYear;
            Power = power;
            Drivetrain = drivetrain;
            Engine = engine;
            EnginePosition = enginePosition;
            TransmissionType = transmissionType;
            TopSpeed = topspeed;
            Doors = doors;
            Price = price;
            ShortDescription = shortDescription;
            FullDescription = fullDescription;
            Images = images;
            VideoLink = videoLink;
        }

        public Car(CarViewModel carViewModel)
        {
            Brand = carViewModel.Brand;
            Model = carViewModel.Model;
            ManufacturingYear = carViewModel.ManufacturingYear;
            ModelYear = carViewModel.ModelYear;
            Power = carViewModel.Power;
            Drivetrain = carViewModel.Drivetrain;
            Engine = carViewModel.Engine;
            EnginePosition = carViewModel.EnginePosition;
            TransmissionType = carViewModel.TransmissionType;
            TopSpeed = carViewModel.TopSpeed;
            Doors = carViewModel.Doors;
            Price = carViewModel.Price;
            ShortDescription = carViewModel.ShortDescription;
            FullDescription = carViewModel.FullDescription;
            VideoLink = carViewModel.VideoLink;

            Images = new List<Image>();
        }
    }
}
