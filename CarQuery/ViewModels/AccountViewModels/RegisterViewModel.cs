using System.ComponentModel.DataAnnotations;

namespace CarQuery.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Informe um nome")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo {1} caracteres")]
        [MaxLength(25, ErrorMessage = "O nome não deve ultrapassar {1} caracteres")]
        [Display(Name = "Nome de usuário")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "É necessário informar o email")]
        [EmailAddress(ErrorMessage = "Informe um email válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe uma senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirme a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; }
    }
}
