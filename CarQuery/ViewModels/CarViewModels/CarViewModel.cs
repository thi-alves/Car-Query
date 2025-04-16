using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarQuery.ViewModels.CarViewModels
{
    public class CarViewModel
    {
        [Required(ErrorMessage = "Informe a marca do veículo")]
        [MinLength(1, ErrorMessage = "A marca do carro deve ter no mínimo {1} caracteres")]
        [MaxLength(30, ErrorMessage = "A marca do carro deve deve ter no máximo {1} caracteres")]
        [Display(Name = "Marca")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Informe o modelo do veículo")]
        [MinLength(1, ErrorMessage = "O modelo deve ter no mínimo {1} caracteres")]
        [MaxLength(100, ErrorMessage = "O modelo deve ter no máximo {1} caracteres")]
        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Informe o tipo de carroceria do carro")]
        [MaxLength(30, ErrorMessage = "Esse campo não deve possuir mais de {1} caracteres")]
        public string BodyStyle { get; set; }

        [Required(ErrorMessage = "Informe o ano de fabricação do veículo")]
        [Display(Name = "Ano de fabricação")]
        public int ManufacturingYear { get; set; }

        [Required(ErrorMessage = "Informe o ano do modelo do veículo")]
        [Display(Name = "Ano do modelo")]
        public int ModelYear { get; set; }

        [Required(ErrorMessage = "Informe a potência do veículo")]
        [Display(Name = "Potência")]
        public short Power { get; set; }

        [Required(ErrorMessage = "Informe a tração do carro")]
        [MaxLength(45, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Tração")]
        public string Drivetrain { get; set; }

        [Required(ErrorMessage = "Especifique a cilindrada em litros")]
        [MaxLength(6, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Cilindrada (L)")]
        public string Displacement { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de válvulas")]
        [Display(Name = "Válvulas")]
        public int Valves { get; set; }

        [Required(ErrorMessage = "Informe o tipo de combustível")]
        [MaxLength(25, ErrorMessage = "Esse campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Combustível")]
        public string FuelType { get; set; }

        [Required(ErrorMessage = "Informe o tipo de aspiração")]
        [MaxLength(30, ErrorMessage = "Este campo não deve possuir mais de {1} caracteres")]
        [Display(Name = "Aspiração")]
        public string Aspiration { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de cilindros")]
        [Display(Name = "Cilindros")]
        public int Cylinders { get; set; }

        [Required(ErrorMessage = "Informe a configuração dos cilindros")]
        [MaxLength(30, ErrorMessage = "Esse campo não deve possuir mais de {1} caracteres")]
        public string CylinderConfiguration { get; set; }

        [Required(ErrorMessage = "Informme a posição do motor")]
        [MaxLength(10, ErrorMessage = "A posição do motor deve ter no máximo {1} caracteres")]
        [Display(Name = "Posição do motor")]
        public string EnginePosition { get; set; }

        [Required(ErrorMessage = "Informe o tipo de transmissão")]
        [MaxLength(30, ErrorMessage = "Este campo deve ter no máximo {1} caracteres")]
        [Display(Name = "Transmissão")]
        public string TransmissionType { get; set; }

        [Required(ErrorMessage = "Informe a velocidade máxima do veículo")]
        [Display(Name = "Velocidade máxima")]
        public short TopSpeed { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de portas")]
        [Display(Name = "Portas")]
        public short Doors { get; set; }

        [Required(ErrorMessage = "Informe o preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Preço")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição curta")]
        [MinLength(20, ErrorMessage = "A descrição curta precisa ter no mínimo {1} caracteres")]
        [MaxLength(300, ErrorMessage = "A descrição curta deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição curta")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Escreva uma descrição completa")]
        [MinLength(20, ErrorMessage = "A descrição precisa ter no mínimo {1} caracteres")]
        [MaxLength(5100, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição completa")]
        public string FullDescription { get; set; }

        [Required(ErrorMessage = "Selecione uma imagem")]
        public List<IFormFile> Images { get; set; }

        public string VideoLink { get; set; }
    }
}
