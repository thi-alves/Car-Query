using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarQuery.ViewModels
{
    public class CarViewModel
    {
        [Required]
        [MinLength(2, ErrorMessage = "A marca do carro deve ter no mínimo {1} caracteres")]
        [MaxLength(30, ErrorMessage = "A marca do carro deve ter no máximo {1} caracteres")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required]
        [MinLength(11, ErrorMessage = "O nome modelo deve ter no mínimo {1} caracteres")]
        [MaxLength(100, ErrorMessage = "O nome modelo deve ter no máximo {1} caracteres")]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Informe o ano do veículo")]
        [MaxLength(9, ErrorMessage = "Este campo deve ter no máximo 9 caracteres")]
        [Display(Name ="Year")]
        public string Year { get; set; }

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

        [Required(ErrorMessage = "Informe a posição do motor")]
        [MaxLength(8, ErrorMessage = "A posição do motor deve ter no máximo {1} caracteres")]
        [Display(Name = "Engine position")]
        public string EnginePosition { get; set; }

        [Required(ErrorMessage = "Informe o tipo de transmissão")]
        [MaxLength(20, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Transmission type")]
        public string TransmissionType { get; set; }

        [Required(ErrorMessage = "Informe a velocidade máxima do veículo")]
        [Display(Name = "Top speed")]
        public short TopSpeed { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de portas")]
        [Display(Name= "Doors")]
        public short Doors { get; set; }

        [Required(ErrorMessage = "Informe o preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição curta")]
        [MinLength(20, ErrorMessage = "A descrição curta precisa ter no mínimo {1} caracteres")]
        [MaxLength(200, ErrorMessage = "A descrição curta deve ter no máximo {1} caracteres")]
        [Display(Name = "Short description")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição completa")]
        [MinLength(20, ErrorMessage = "A descrição precisa ter no mínimo {1} caracteres")]
        [MaxLength(2100, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Full description")]
        public string FullDescription { get; set; }

        public List<IFormFile> Images { get; set; }
        public string VideoLink { get; set; }
    }
}
